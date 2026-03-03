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
                    // Only one player has selected - just acknowledge to the caller
                    // The result already has the correct perspective for the caller
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
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing ability selection for battle {battleId}");
                await Clients.Caller.SendAsync("Error", ex.Message);
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
