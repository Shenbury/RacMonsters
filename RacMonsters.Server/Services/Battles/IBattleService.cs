using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Battles
{
    public interface IBattleService
    {
        Task<Battle> CreateBattle(Battle battle);
        Task<Battle?> GetBattle(int id);

        // Multiplayer methods - Phase 4
        Task<int> CreateMultiplayerBattle(
            string player1ConnectionId,
            int player1CharacterId,
            string player2ConnectionId,
            int player2CharacterId);

        Task<BattleResult> ProcessPlayerMove(int battleId, string connectionId, int abilityId);
        Task<bool> IsPlayersTurn(int battleId, string connectionId);
        Task HandlePlayerDisconnect(int battleId, string connectionId);
        Task<List<Battle>> GetBattlesWithExpiredTurns();
        Task ProcessAutoMove(int battleId);
        Task EndBattleByForfeit(int battleId, string winnerConnectionId);

        // Team Battle methods - Phase 2
        Task<int> CreateTeamBattle(
            string player1ConnectionId,
            List<int> player1CharacterIds,
            string player2ConnectionId,
            List<int> player2CharacterIds);

        Task<BattleResult> ProcessSwitchCharacter(int battleId, string connectionId, int newCharacterIndex);
    }
}
