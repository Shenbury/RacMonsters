using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Abilities
{
    public class AbilityRepository : IAbilityRepository
    {
        public async Task<Ability[]> GetAbilities(int[] ids)
        {
            // Simple in-memory abilities store
            var abilities = new[]
            {
                new Ability { Id = 1, Name = "Tackle", Description = "A basic physical attack.", IsTech = false, Power = 5, Speed = 5, Accuracy = 0.95 },
                new Ability { Id = 2, Name = "Ember", Description = "A basic tech fire attack.", IsTech = true, Power = 7, Speed = 6, Accuracy = 0.9 },
                new Ability { Id = 3, Name = "Heal", Description = "Restore some health.", IsTech = true, IsHeal = true, Power = 8, Speed = 4, Accuracy = 1.0 }
            };

            var result = abilities.Where(a => ids.Contains(a.Id)).ToArray();
            return await Task.FromResult(result);
        }
    }
}
