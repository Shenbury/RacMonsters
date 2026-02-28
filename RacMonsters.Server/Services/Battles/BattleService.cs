using RacMonsters.Server.Repositories.Battles;
using RacMonsters.Server.Repositories.Characters;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Battles
{
    public class BattleService : IBattleService
    {
        private readonly IBattleRepository _battleRepository;
        private readonly ICharacterRepository _characterRepository;

        public BattleService(IBattleRepository battleRepository, ICharacterRepository characterRepository)
        {
            _battleRepository = battleRepository;
            _characterRepository = characterRepository;
        }

        public async Task<Battle> CreateBattle(Battle battle)
        {
            // assume caller supplied characters or they are already resolved
            battle.Rounds ??= Array.Empty<Round>();
            return await _battleRepository.CreateBattle(battle);
        }

        public async Task<Battle?> GetBattle(int id)
        {
            return await _battleRepository.GetBattle(id);
        }
    }
}
