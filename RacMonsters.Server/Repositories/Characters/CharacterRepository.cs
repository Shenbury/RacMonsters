using Microsoft.EntityFrameworkCore;
using RacMonsters.Server.Data;
using RacMonsters.Server.Models;
using System.Runtime.InteropServices;

namespace RacMonsters.Server.Repositories.Characters
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly RacMonstersDbContext _db;

        public CharacterRepository(RacMonstersDbContext db)
        {
            _db = db;
        }

        private Ability[] MapAbilities(IEnumerable<CharacterAbilityEntity> characterAbilities)
        {
            var abilities = characterAbilities.Select(ca => new Ability
            {
                Id = ca.Ability!.Id,
                Name = ca.Ability.Name,
                Description = ca.Ability.Description,
                IsTech = ca.Ability.IsTech,
                IsHeal = ca.Ability.IsHeal,
                Power = ca.Ability.Power,
                Speed = ca.Ability.Speed,
                Accuracy = ca.Ability.Accuracy,
                StatusEffects = ca.Ability.StatusEffects?.Select(se => new StatusEffectApplication
                {
                    Type = (StatusEffectType)se.StatusEffectType,
                    Duration = se.Duration,
                    Power = se.Power,
                    Modifier = se.Modifier,
                    ApplyChance = se.ApplyChance,
                    ApplyToSelf = se.ApplyToSelf,
                    RequiresCharging = se.RequiresCharging
                }).ToList() ?? new List<StatusEffectApplication>()
            }).ToArray();

            return abilities;
        }

        public async Task<Character> GetCharacter(int id)
        {
            var ent = await _db.Characters
                .Include(c => c.CharacterAbilities)
                    .ThenInclude(ca => ca.Ability)
                        .ThenInclude(a => a!.StatusEffects)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (ent == null) return null!;

            return new Character
            {
                Id = ent.Id,
                Name = ent.Name,
                ImageUrl = ent.ImageUrl,
                MaxHealth = ent.MaxHealth,
                CurrentHealth = ent.CurrentHealth,
                Attack = ent.Attack,
                Defense = ent.Defense,
                TechAttack = ent.TechAttack,
                TechDefense = ent.TechDefense,
                Abilities = MapAbilities(ent.CharacterAbilities)
            };
        }

        public async Task<Character> GetRandomCharacter()
        {
            var ent = await _db.Characters
                .Include(c => c.CharacterAbilities)
                    .ThenInclude(ca => ca.Ability)
                        .ThenInclude(a => a!.StatusEffects)
                .OrderBy(c => c.Id)
                .ToArrayAsync();

            if (ent.Length == 0) return null!;

            var r = new Random();
            var picked = ent[r.Next(ent.Length)];

            return await GetCharacter(picked.Id);
        }

        public async Task<Character[]> GetAll()
        {
            var ents = await _db.Characters
                .Include(c => c.CharacterAbilities)
                    .ThenInclude(ca => ca.Ability)
                        .ThenInclude(a => a!.StatusEffects)
                .ToArrayAsync();

            return ents.Select(ent => new Character
            {
                Id = ent.Id,
                Name = ent.Name,
                ImageUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? ent.ImageUrl.Split('/').Last() : ent.ImageUrl,
                MaxHealth = ent.MaxHealth,
                CurrentHealth = ent.CurrentHealth,
                Attack = ent.Attack,
                Defense = ent.Defense,
                TechAttack = ent.TechAttack,
                TechDefense = ent.TechDefense,
                Abilities = MapAbilities(ent.CharacterAbilities)
            }).ToArray();
        }
    }
}
