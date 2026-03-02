using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Abilities;
using System.Linq;
using System.Runtime.InteropServices;


namespace RacMonsters.Server.Repositories.Characters
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IAbilityRepository _abilityRepository;

        public CharacterRepository(IAbilityRepository abilityRepository)
        {
            _abilityRepository = abilityRepository;
        }

        private record CharacterTemplate
        (
            int Id,
            string Name,
            string ImageUrl,
            int MaxHealth,
            int CurrentHealth,
            int Attack,
            int Defense,
            int TechAttack,
            int TechDefense,
            int[] AbilityIds
        );

        private readonly CharacterTemplate[] _templates = new[]
        {
            new CharacterTemplate(
            Id: 1,
            Name: "Ashley",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Ashley.png" : "public/Ashley.png",
            MaxHealth: 50,
            CurrentHealth: 50,
            Attack: 6,
            Defense: 6,
            TechAttack: 5,
            TechDefense: 5,
            AbilityIds: new[] { 1, 2, 3, 4 }
            ),
            new CharacterTemplate(
            Id: 2,
            Name: "Banbury",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Banbury.png" : "public/Banbury.png",
            MaxHealth: 35,
            CurrentHealth: 35,
            Attack: 5,
            Defense: 5,
            TechAttack: 11,
            TechDefense: 7,
            AbilityIds: new[] { 5, 6, 7, 8 }
            ),
            new CharacterTemplate(
            Id: 3,
            Name: "Beattie",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Beattie.png" : "public/Beattie.png",
            MaxHealth: 65,
            CurrentHealth: 65,
            Attack: 2,
            Defense: 8,
            TechAttack: 2,
            TechDefense: 5,
            AbilityIds: new[] { 9, 10, 11, 12 }
            ),
            new CharacterTemplate(
            Id: 4,
            Name: "Faisal",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Faisal.png" : "public/Faisal.png",
            MaxHealth: 60,
            CurrentHealth: 60,
            Attack: 1,
            Defense: 8,
            TechAttack: 3,
            TechDefense: 8,
            AbilityIds: new[] { 13, 14, 15, 16 }
            ),
            new CharacterTemplate(
            Id: 5,
            Name: "JP",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "JP.png" : "public/JP.png",
            MaxHealth: 45,
            CurrentHealth: 45,
            Attack: 8,
            Defense: 8,
            TechAttack: 6,
            TechDefense: 7,
            AbilityIds: new[] { 17, 18, 19, 20 }
            ),
            new CharacterTemplate(
            Id: 6,
            Name: "Langdon",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Langdon.png" : "public/Langdon.png",
            MaxHealth: 40,
            CurrentHealth: 40,
            Attack: 6,
            Defense: 10,
            TechAttack: 2,
            TechDefense: 6,
            AbilityIds: new[] { 21, 22, 23, 24 }
            ),
            new CharacterTemplate(
            Id: 7,
            Name: "Lilley",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Lilley.png" : "public/Lilley.png",
            MaxHealth: 40,
            CurrentHealth: 40,
            Attack: 8,
            Defense: 6,
            TechAttack: 4,
            TechDefense: 4,
            AbilityIds: new[] { 25, 26, 27, 28 }
            ),
            new CharacterTemplate(
            Id: 8,
            Name: "Nunan",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Nunan.png" : "public/Nunan.png",
            MaxHealth: 30,
            CurrentHealth: 30,
            Attack: 2,
            Defense: 2,
            TechAttack: 12,
            TechDefense: 8,
            AbilityIds: new[] { 29, 30, 31, 32 }
            ),
            new CharacterTemplate(
            Id: 9,
            Name: "Sam",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Sam.png" : "public/Sam.png",
            MaxHealth: 40,
            CurrentHealth: 32,
            Attack: 12,
            Defense: 7,
            TechAttack: 5,
            TechDefense: 3,
            AbilityIds: new[] { 33, 34, 35, 36 }
            ),
            new CharacterTemplate(
            Id: 10,
            Name: "Simon",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Simon.png" : "public/Simon.png",
            MaxHealth: 40,
            CurrentHealth: 40,
            Attack: 5,
            Defense: 9,
            TechAttack: 10,
            TechDefense: 8,
            AbilityIds: new[] { 37, 38, 39 ,40 }
            ),
            new CharacterTemplate(
            Id: 11,
            Name: "Paul",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Paul2.png" : "public/Paul2.png",
            MaxHealth: 35,
            CurrentHealth: 35,
            Attack: 8,
            Defense: 8,
            TechAttack: 12,
            TechDefense: 7,
            AbilityIds: new[] { 41, 42, 43 ,44 }
            ),
            new CharacterTemplate(
            Id: 12,
            Name: "Charl",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Charl.png" : "public/Charl.png",
            MaxHealth: 30,
            CurrentHealth: 30,
            Attack: 6,
            Defense: 8,
            TechAttack: 5,
            TechDefense: 12,
            AbilityIds: new[] { 45, 46, 47 ,48 }
            ),
            new CharacterTemplate(
            Id: 13,
            Name: "Barnes",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Barnes.png" : "public/Barnes.png",
            MaxHealth: 35,
            CurrentHealth: 35,
            Attack: 7,
            Defense: 8,
            TechAttack: 4,
            TechDefense: 4,
            AbilityIds: new[] { 49, 50, 51 ,52 }
            ),
            new CharacterTemplate(
            Id: 14,
            Name: "Belassie",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Belassie.png" : "public/Belassie.png",
            MaxHealth: 20,
            CurrentHealth: 20,
            Attack: 12,
            Defense: 10,
            TechAttack: 2,
            TechDefense: 2,
            AbilityIds: new[] { 53, 54, 55 ,56 }
            ),
            new CharacterTemplate(
            Id: 15,
            Name: "Sailesh",
            ImageUrl: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Sailesh.png" : "public/Sailesh.png",
            MaxHealth: 50,
            CurrentHealth: 50,
            Attack: 7,
            Defense: 4,
            TechAttack: 10,
            TechDefense: 8,
            AbilityIds: new[] { 57, 58, 59 ,60 }
            )
        };

        public async Task<Character> GetCharacter(int id)
        {
            var tpl = _templates.FirstOrDefault(t => t.Id == id);
            if (tpl == null) return await Task.FromResult<Character?>(null!);

            var abilities = await _abilityRepository.GetAbilities(tpl.AbilityIds);
            var character = new Character
            {
                Id = tpl.Id,
                Name = tpl.Name,
                ImageUrl = tpl.ImageUrl,
                MaxHealth = tpl.MaxHealth,
                CurrentHealth = tpl.CurrentHealth,
                Attack = tpl.Attack,
                Defense = tpl.Defense,
                TechAttack = tpl.TechAttack,
                TechDefense = tpl.TechDefense,
                Abilities = abilities
            };

            return character;
        }

        public async Task<Character> GetRandomCharacter()
        {
            var r = new Random();
            var tpl = _templates[r.Next(_templates.Length)];
            return await GetCharacter(tpl.Id);
        }

        public async Task<Character[]> GetAll()
        {
            // gather all ability ids and fetch once
            var allIds = _templates.SelectMany(t => t.AbilityIds).Distinct().ToArray();
            var abilities = await _abilityRepository.GetAbilities(allIds);
            var abilityById = abilities.ToDictionary(a => a.Id);

            var chars = _templates.Select(t => new Character
            {
                Id = t.Id,
                Name = t.Name,
                ImageUrl = t.ImageUrl,
                MaxHealth = t.MaxHealth,
                CurrentHealth = t.CurrentHealth,
                Attack = t.Attack,
                Defense = t.Defense,
                TechAttack = t.TechAttack,
                TechDefense = t.TechDefense,
                Abilities = t.AbilityIds.Select(id => abilityById.TryGetValue(id, out var a) ? a : null)
                    .Where(a => a != null).Cast<Ability>().ToArray()
            }).ToArray();

            return chars;
        }
    }
}
