using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Abilities
{
    public interface IAbilityService
    {
        Task<Ability[]> GetAbilities(int[] ids);
    }
}
