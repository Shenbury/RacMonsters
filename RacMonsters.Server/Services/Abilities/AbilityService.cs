using RacMonsters.Server.Repositories.Abilities;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Abilities
{
    public class AbilityService : IAbilityService
    {
        private readonly IAbilityRepository _abilityRepository;

        public AbilityService(IAbilityRepository abilityRepository)
        {
            _abilityRepository = abilityRepository;
        }

        public async Task<Ability[]> GetAbilities(int[] ids)
        {
            return await _abilityRepository.GetAbilities(ids);
        }
    }
}
