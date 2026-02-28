using RacMonsters.Server.Repositories.Battles;

namespace RacMonsters.Server.Services.Battles
{
    public class BattleService : IBattleService
    {
        private readonly IBattleRepository _battleRepository;

        public BattleService(IBattleRepository battleRepository)
        {
            _battleRepository = battleRepository;
        }
    }
}
