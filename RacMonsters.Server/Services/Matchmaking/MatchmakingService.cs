using Microsoft.AspNetCore.SignalR;
using RacMonsters.Server.Hubs;
using RacMonsters.Server.Models;
using RacMonsters.Server.Services.Battles;
using System.Collections.Concurrent;

namespace RacMonsters.Server.Services.Matchmaking
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly ConcurrentQueue<MatchmakingPlayer> _queue = new();
        private readonly ConcurrentDictionary<string, MatchmakingPlayer> _playerLookup = new();
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MatchmakingService> _logger;
        private readonly SemaphoreSlim _matchmakingSemaphore = new(1, 1);

        public MatchmakingService(
            IHubContext<GameHub> hubContext,
            IServiceProvider serviceProvider,
            ILogger<MatchmakingService> logger)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task AddPlayerToQueue(string connectionId, string playerName, int characterId)
        {
            var player = new MatchmakingPlayer
            {
                ConnectionId = connectionId,
                PlayerName = playerName,
                CharacterId = characterId,
                JoinedAt = DateTime.UtcNow,
                IsTeamBattle = false
            };

            // Check if player already in queue
            if (_playerLookup.ContainsKey(connectionId))
            {
                _logger.LogWarning($"Player {playerName} ({connectionId}) already in queue");
                return;
            }

            _queue.Enqueue(player);
            _playerLookup.TryAdd(connectionId, player);

            _logger.LogInformation($"Player {playerName} joined matchmaking queue. Queue size: {_queue.Count}");

            await _hubContext.Clients.Client(connectionId).SendAsync(
                "MatchmakingStatus", 
                "Searching for opponent...", 
                _queue.Count
            );

            await TryMatchPlayers();
        }

        public async Task AddTeamToQueue(string connectionId, string playerName, List<int> characterIds)
        {
            if (characterIds.Count != 4)
            {
                _logger.LogWarning($"Player {playerName} tried to join team queue with {characterIds.Count} characters instead of 4");
                await _hubContext.Clients.Client(connectionId).SendAsync(
                    "MatchmakingError",
                    "Team battles require exactly 4 characters."
                );
                return;
            }

            var player = new MatchmakingPlayer
            {
                ConnectionId = connectionId,
                PlayerName = playerName,
                JoinedAt = DateTime.UtcNow,
                IsTeamBattle = true,
                TeamCharacterIds = characterIds,
                CharacterId = characterIds[0] // Set first character as primary for compatibility
            };

            // Check if player already in queue
            if (_playerLookup.ContainsKey(connectionId))
            {
                _logger.LogWarning($"Player {playerName} ({connectionId}) already in queue");
                return;
            }

            _queue.Enqueue(player);
            _playerLookup.TryAdd(connectionId, player);

            _logger.LogInformation($"Player {playerName} joined team battle matchmaking queue. Queue size: {_queue.Count}");

            await _hubContext.Clients.Client(connectionId).SendAsync(
                "MatchmakingStatus", 
                "Searching for team battle opponent...", 
                _queue.Count
            );

            await TryMatchPlayers();
        }

        private async Task TryMatchPlayers()
        {
            await _matchmakingSemaphore.WaitAsync();
            try
            {
                // Keep trying to match as long as we have at least 2 players
                bool matchedThisRound = true;

                while (_queue.Count >= 2 && matchedThisRound)
                {
                    matchedThisRound = false;

                    // Get all players from queue into a temporary list
                    var tempList = new List<MatchmakingPlayer>();
                    while (_queue.TryDequeue(out var player))
                    {
                        tempList.Add(player);
                    }

                    // Try to find compatible pairs
                    for (int i = 0; i < tempList.Count - 1; i++)
                    {
                        if (tempList[i] == null) continue;

                        for (int j = i + 1; j < tempList.Count; j++)
                        {
                            if (tempList[j] == null) continue;

                            var player1 = tempList[i];
                            var player2 = tempList[j];

                            // Check if both players are in the same mode
                            if (player1.IsTeamBattle == player2.IsTeamBattle)
                            {
                                // Found a match!
                                matchedThisRound = true;

                                // Remove from lookup
                                _playerLookup.TryRemove(player1.ConnectionId, out _);
                                _playerLookup.TryRemove(player2.ConnectionId, out _);

                                // Mark as null so they won't be re-queued
                                tempList[i] = null!;
                                tempList[j] = null!;

                                _logger.LogInformation($"Match found: {player1.PlayerName} vs {player2.PlayerName} ({(player1.IsTeamBattle ? "Team Battle" : "Standard")})");

                                // Create battle using scoped service
                                using var scope = _serviceProvider.CreateScope();
                                var battleService = scope.ServiceProvider.GetRequiredService<IBattleService>();

                                try
                                {
                                    int battleId;

                                    if (player1.IsTeamBattle && player1.TeamCharacterIds != null && 
                                        player2.IsTeamBattle && player2.TeamCharacterIds != null)
                                    {
                                        // Create team battle
                                        battleId = await battleService.CreateTeamBattle(
                                            player1.ConnectionId,
                                            player1.TeamCharacterIds,
                                            player2.ConnectionId,
                                            player2.TeamCharacterIds
                                        );

                                        _logger.LogInformation($"Team Battle {battleId} created: {player1.PlayerName} vs {player2.PlayerName}");
                                    }
                                    else
                                    {
                                        // Create standard 1v1 battle
                                        battleId = await battleService.CreateMultiplayerBattle(
                                            player1.ConnectionId,
                                            player1.CharacterId,
                                            player2.ConnectionId,
                                            player2.CharacterId
                                        );

                                        _logger.LogInformation($"Standard Battle {battleId} created: {player1.PlayerName} vs {player2.PlayerName}");
                                    }

                                    // Add both players to battle group
                                    await _hubContext.Groups.AddToGroupAsync(player1.ConnectionId, $"battle-{battleId}");
                                    await _hubContext.Groups.AddToGroupAsync(player2.ConnectionId, $"battle-{battleId}");

                                    // Get the created battle to send complete team data
                                    var createdBattle = await battleService.GetBattle(battleId);

                                    if (player1.IsTeamBattle && createdBattle?.Mode == BattleMode.TeamBattle)
                                    {
                                        // Team Battle - send team data
                                        await _hubContext.Clients.Client(player1.ConnectionId).SendAsync(
                                            "MatchFound",
                                            battleId,
                                            player2.PlayerName,
                                            player2.CharacterId, // Keep for backward compatibility
                                            true, // isMyTurn
                                            createdBattle.Team2Characters // opponentTeam
                                        );

                                        await _hubContext.Clients.Client(player2.ConnectionId).SendAsync(
                                            "MatchFound",
                                            battleId,
                                            player1.PlayerName,
                                            player1.CharacterId, // Keep for backward compatibility
                                            false, // isMyTurn
                                            createdBattle.Team1Characters // opponentTeam
                                        );
                                    }
                                    else
                                    {
                                        // Standard 1v1 Battle
                                        await _hubContext.Clients.Client(player1.ConnectionId).SendAsync(
                                            "MatchFound",
                                            battleId,
                                            player2.PlayerName,
                                            player2.CharacterId,
                                            true // isMyTurn
                                        );

                                        await _hubContext.Clients.Client(player2.ConnectionId).SendAsync(
                                            "MatchFound",
                                            battleId,
                                            player1.PlayerName,
                                            player1.CharacterId,
                                            false // isMyTurn
                                        );
                                    }

                                    _logger.LogInformation($"Battle {battleId} created successfully for {player1.PlayerName} vs {player2.PlayerName}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error creating multiplayer battle");

                                    // Notify players of error
                                    await _hubContext.Clients.Client(player1.ConnectionId).SendAsync(
                                        "MatchmakingError",
                                        "Failed to create battle. Please try again."
                                    );
                                    await _hubContext.Clients.Client(player2.ConnectionId).SendAsync(
                                        "MatchmakingError",
                                        "Failed to create battle. Please try again."
                                    );
                                }

                                break; // Break inner loop after finding a match for player1
                            }
                        }
                    }

                    // Re-queue any players who weren't matched
                    foreach (var player in tempList)
                    {
                        if (player != null)
                        {
                            _queue.Enqueue(player);
                        }
                    }
                }
            }
            finally
            {
                _matchmakingSemaphore.Release();
            }
        }

        public async Task RemovePlayer(string connectionId)
        {
            if (_playerLookup.TryRemove(connectionId, out var player))
            {
                _logger.LogInformation($"Removing player {player.PlayerName} ({connectionId}) from matchmaking queue");

                // Rebuild queue without the disconnected player
                var tempList = new List<MatchmakingPlayer>();
                while (_queue.TryDequeue(out var queuedPlayer))
                {
                    if (queuedPlayer.ConnectionId != connectionId)
                    {
                        tempList.Add(queuedPlayer);
                    }
                }

                // Re-enqueue remaining players
                foreach (var remainingPlayer in tempList)
                {
                    _queue.Enqueue(remainingPlayer);
                }

                _logger.LogInformation($"Player removed. Queue size: {_queue.Count}");
            }

            await Task.CompletedTask;
        }

        public Task<int> GetQueueSize()
        {
            return Task.FromResult(_queue.Count);
        }

        public async Task<List<MatchmakingPlayer>> GetQueuedPlayers()
        {
            return await Task.FromResult(_playerLookup.Values.ToList());
        }

        public async Task ClearQueue()
        {
            _logger.LogWarning("Clearing matchmaking queue");
            
            while (_queue.TryDequeue(out _)) { }
            _playerLookup.Clear();

            await Task.CompletedTask;
        }
    }
}
