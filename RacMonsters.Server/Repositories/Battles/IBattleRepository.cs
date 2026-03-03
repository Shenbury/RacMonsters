using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Battles
{
    public interface IBattleRepository
    {
        Task<Battle> CreateBattle(Battle createBattle);
        Task<Battle> GetBattle(int battleId);
        Task<Battle> UpdateBattle(Battle battle);
        Task<List<Battle>> GetBattlesWithExpiredTurns();
    }
}
