using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Battles
{
    public interface IBattleService
    {
        Task<Battle> CreateBattle(Battle battle);
        Task<Battle?> GetBattle(int id);
    }
}
