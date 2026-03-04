using Microsoft.AspNetCore.SignalR;
using RacMonsters.Server.Services.Battles;
using RacMonsters.Server.Services.Matchmaking;

namespace RacMonsters.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMatchmakingService _matchmaking;
        private readonly IBattleService _battleService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(
            IMatchmakingService matchmaking, 
            IBattleService battleService,
            ILogger<GameHub> logger)
        {
            _matchmaking = matchmaking;
            _battleService = battleService;
            _logger = logger;
        }

        public async Task JoinMatchmaking(string playerName, int characterId)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Player {playerName} (Connection: {connectionId}) joining matchmaking with character {characterId}");

            await _matchmaking.AddPlayerToQueue(connectionId, playerName, characterId);
        }

        // Team Battle - Phase 4
        public async Task JoinTeamMatchmaking(string playerName, List<int> characterIds)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Player {playerName} (Connection: {connectionId}) joining team matchmaking with characters {string.Join(",", characterIds)}");

            await _matchmaking.AddTeamToQueue(connectionId, playerName, characterIds);
        }

        public async Task SelectAbility(int battleId, int abilityId)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Player {connectionId} selecting ability {abilityId} in battle {battleId}");

            try
            {
                var result = await _battleService.ProcessPlayerMove(battleId, connectionId, abilityId);

                // Get the battle to determine player positions
                var battle = await _battleService.GetBattle(battleId);
                if (battle == null)
                {
                    await Clients.Caller.SendAsync("Error", "Battle not found");
                    return;
                }

                // Check if a round was processed (LastRound will be populated) or just waiting
                if (result.LastRound != null)
                {
                    // Round was processed - send result to both players with correct character perspectives

                    // Player 1 perspective
                    var player1Result = new 
                    {
                        battleId = result.BattleId,
                        playerCharacter = result.PlayerCharacter, // This is always CharacterA (Player 1's character)
                        opponentCharacter = result.OpponentCharacter, // This is always CharacterB (Player 2's character)
                        lastRound = result.LastRound,
                        isGameOver = result.IsGameOver,
                        winner = result.Winner,
                        message = result.Message,
                        triggeringPlayerConnectionId = result.TriggeringPlayerConnectionId,
                        triggeringPlayerName = result.TriggeringPlayerName
                    };

                    // Player 2 perspective (swap the characters)
                    var player2Result = new 
                    {
                        battleId = result.BattleId,
                        playerCharacter = result.OpponentCharacter, // For Player 2, their character is CharacterB
                        opponentCharacter = result.PlayerCharacter, // For Player 2, opponent is CharacterA
                        lastRound = result.LastRound,
                        isGameOver = result.IsGameOver,
                        winner = result.Winner,
                        message = result.Message,
                        triggeringPlayerConnectionId = result.TriggeringPlayerConnectionId,
                        triggeringPlayerName = result.TriggeringPlayerName
                    };

                    // Send to Player 1
                    await Clients.Client(battle.Player1ConnectionId!).SendAsync("TurnProcessed", player1Result);

                    // Send to Player 2
                    await Clients.Client(battle.Player2ConnectionId!).SendAsync("TurnProcessed", player2Result);
                }
                else
                {
                    // Only one player has selected - acknowledge to the caller and notify opponent
                    var isPlayer1 = connectionId == battle.Player1ConnectionId;

                    // Send acknowledgement to the player who just selected
                    await Clients.Caller.SendAsync("TurnProcessed", new 
                    {
                        battleId = result.BattleId,
                        playerCharacter = result.PlayerCharacter,
                        opponentCharacter = result.OpponentCharacter,
                        lastRound = result.LastRound,
                        isGameOver = result.IsGameOver,
                        winner = result.Winner,
                        message = result.Message,
                        triggeringPlayerConnectionId = result.TriggeringPlayerConnectionId,
                        triggeringPlayerName = result.TriggeringPlayerName
                    });

                    // Notify the opponent that the other player is waiting
                    var opponentConnectionId = isPlayer1 ? battle.Player2ConnectionId : battle.Player1ConnectionId;
                    var opponentMessage = $"{result.TriggeringPlayerName} has selected their ability. Waiting for you...";

                    await Clients.Client(opponentConnectionId!).SendAsync("OpponentReady", new
                    {
                        battleId = result.BattleId,
                        message = opponentMessage,
                        opponentName = result.TriggeringPlayerName
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing ability selection for battle {battleId}");
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        // Team Battle - Switch Character (Phase 4)
        public async Task SwitchCharacter(int battleId, int newCharacterIndex)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Player {connectionId} switching to character index {newCharacterIndex} in battle {battleId}");

            try
            {
                var result = await _battleService.ProcessSwitchCharacter(battleId, connectionId, newCharacterIndex);

                // Get the battle to determine player positions
                var battle = await _battleService.GetBattle(battleId);
                if (battle == null)
                {
                    await Clients.Caller.SendAsync("Error", "Battle not found");
                    return;
                }

                // Check if both players are ready (round processed)
                if (result.LastRound != null || result.IsGameOver)
                {
                    // Round was processed - send result to both players

                    // Determine if this is player 1 or player 2
                    var isPlayer1 = connectionId == battle.Player1ConnectionId;

                    // Player 1 perspective
                    var player1Result = new 
                    {
                        battleId = result.BattleId,
                        playerCharacter = battle.GetActiveTeam1Character(),
                        opponentCharacter = battle.GetActiveTeam2Character(),
                        playerTeam = battle.Team1Characters,
                        opponentTeam = battle.Team2Characters,
                        lastRound = result.LastRound,
                        isGameOver = result.IsGameOver,
                        winner = result.Winner,
                        message = result.Message,
                        triggeringPlayerConnectionId = result.TriggeringPlayerConnectionId,
                        triggeringPlayerName = result.TriggeringPlayerName
                    };

                    // Player 2 perspective
                    var player2Result = new 
                    {
                        battleId = result.BattleId,
                        playerCharacter = battle.GetActiveTeam2Character(),
                        opponentCharacter = battle.GetActiveTeam1Character(),
                        playerTeam = battle.Team2Characters,
                        opponentTeam = battle.Team1Characters,
                        lastRound = result.LastRound,
                        isGameOver = result.IsGameOver,
                        winner = result.Winner,
                        message = result.Message,
                        triggeringPlayerConnectionId = result.TriggeringPlayerConnectionId,
                        triggeringPlayerName = result.TriggeringPlayerName
                    };

                    // Send to Player 1
                    await Clients.Client(battle.Player1ConnectionId!).SendAsync("TurnProcessed", player1Result);

                    // Send to Player 2
                    await Clients.Client(battle.Player2ConnectionId!).SendAsync("TurnProcessed", player2Result);
                }
                else
                {
                    // Only one player has acted - acknowledge and notify opponent
                    var isPlayer1 = connectionId == battle.Player1ConnectionId;
                    var activeChar = isPlayer1 ? battle.GetActiveTeam1Character() : battle.GetActiveTeam2Character();
                    var opponentChar = isPlayer1 ? battle.GetActiveTeam2Character() : battle.GetActiveTeam1Character();

                    // Send acknowledgement to the player who just switched
                    await Clients.Caller.SendAsync("CharacterSwitched", new 
                    {
                        battleId = result.BattleId,
                        playerCharacter = activeChar,
                        opponentCharacter = opponentChar,
                        playerTeam = isPlayer1 ? battle.Team1Characters : battle.Team2Characters,
                        opponentTeam = isPlayer1 ? battle.Team2Characters : battle.Team1Characters,
                        message = result.Message,
                        newCharacterName = activeChar.Name
                    });

                    // Notify the opponent that the other player switched
                    var opponentConnectionId = isPlayer1 ? battle.Player2ConnectionId : battle.Player1ConnectionId;
                    await Clients.Client(opponentConnectionId!).SendAsync("OpponentSwitched", new
                    {
                        battleId = result.BattleId,
                        message = $"Opponent switched characters!",
                        opponentCharacter = activeChar
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing character switch for battle {battleId}");
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task LeaveBattle(int battleId)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Player {connectionId} leaving battle {battleId}");

            try
            {
                // Remove player from battle group
                await Groups.RemoveFromGroupAsync(connectionId, $"battle-{battleId}");

                // Log for debugging
                _logger.LogInformation($"Player {connectionId} removed from battle group {battleId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing player from battle {battleId}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");

            await _matchmaking.RemovePlayer(Context.ConnectionId);

            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected with error");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
