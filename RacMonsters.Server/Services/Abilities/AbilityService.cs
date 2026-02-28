using RacMonsters.Server.Repositories.Abilities;

namespace RacMonsters.Server.Services.Abilities
{
    public class AbilityService : IAbilityService
    {
        private readonly IAbilityRepository _abilityRepository;

        public AbilityService(IAbilityRepository abilityRepository)
        {
            _abilityRepository = abilityRepository;
        }
    }
}
