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
                JoinedAt = DateTime.UtcNow
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

        private async Task TryMatchPlayers()
        {
            await _matchmakingSemaphore.WaitAsync();
            try
            {
                while (_queue.Count >= 2)
                {
                    if (_queue.TryDequeue(out var player1) && _queue.TryDequeue(out var player2))
                    {
                        // Remove from lookup
                        _playerLookup.TryRemove(player1.ConnectionId, out _);
                        _playerLookup.TryRemove(player2.ConnectionId, out _);

                        _logger.LogInformation($"Match found: {player1.PlayerName} (Char {player1.CharacterId}) vs {player2.PlayerName} (Char {player2.CharacterId})");

                        // Create multiplayer battle using scoped service
                        using var scope = _serviceProvider.CreateScope();
                        var battleService = scope.ServiceProvider.GetRequiredService<IBattleService>();

                        try
                        {
                            var battleId = await battleService.CreateMultiplayerBattle(
                                player1.ConnectionId,
                                player1.CharacterId,
                                player2.ConnectionId,
                                player2.CharacterId
                            );

                            // Add both players to battle group
                            await _hubContext.Groups.AddToGroupAsync(player1.ConnectionId, $"battle-{battleId}");
                            await _hubContext.Groups.AddToGroupAsync(player2.ConnectionId, $"battle-{battleId}");

                            // Notify both players that match is found
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
