using Microsoft.EntityFrameworkCore;
using RacMonsters.Server.Data;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Abilities
{
    public class AbilityRepository : IAbilityRepository
    {
        private readonly RacMonstersDbContext _db;

        public AbilityRepository(RacMonstersDbContext db)
        {
            _db = db;
        }

        public async Task<Ability[]> GetAbilities(int[] ids)
        {
            var abilities = await _db.Abilities.Where(a => ids.Contains(a.Id)).ToArrayAsync();
            return abilities.Select(a => new Ability
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                IsTech = a.IsTech,
                IsHeal = a.IsHeal,
                Power = a.Power,
                Speed = a.Speed,
                Accuracy = a.Accuracy
            }).ToArray();
        }
    }
}
