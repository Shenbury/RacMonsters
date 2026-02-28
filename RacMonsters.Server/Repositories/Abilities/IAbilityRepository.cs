using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Abilities
{
    public interface IAbilityRepository
    {
        Task<Ability[]> GetAbilities(int[] ids);
    }
}
