using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Abilities;
using System.Linq;


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

        private static readonly CharacterTemplate[] _templates = new[]
        {
            new CharacterTemplate(
            Id: 1,
            Name: "Ashley",
            ImageUrl: "public/Ashley.png",
            MaxHealth: 50,
            CurrentHealth: 50,
            Attack: 6,
            Defense: 6,
            TechAttack: 3,
            TechDefense: 5,
            AbilityIds: new[] { 1, 2, 3, 4 }
            ),
            new CharacterTemplate(
            Id: 2,
            Name: "Banbury",
            ImageUrl: "public/Banbury.png",
            MaxHealth: 35,
            CurrentHealth: 35,
            Attack: 5,
            Defense: 3,
            TechAttack: 11,
            TechDefense: 7,
            AbilityIds: new[] { 5, 6, 7, 8 }
            ),
            new CharacterTemplate(
            Id: 3,
            Name: "Beattie",
            ImageUrl: "public/Beattie.png",
            MaxHealth: 75,
            CurrentHealth: 75,
            Attack: 2,
            Defense: 8,
            TechAttack: 2,
            TechDefense: 5,
            AbilityIds: new[] { 9, 10, 11, 12 }
            ),
            new CharacterTemplate(
            Id: 4,
            Name: "Faisal",
            ImageUrl: "public/Faisal.png",
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
            ImageUrl: "public/JP.png",
            MaxHealth: 45,
            CurrentHealth: 45,
            Attack: 5,
            Defense: 5,
            TechAttack: 5,
            TechDefense: 5,
            AbilityIds: new[] { 17, 18, 19, 20 }
            ),
            new CharacterTemplate(
            Id: 6,
            Name: "Langdon",
            ImageUrl: "public/Langdon.png",
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
            ImageUrl: "public/Lilley.png",
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
            ImageUrl: "public/Nunan.png",
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
            ImageUrl: "public/Sam.png",
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
            ImageUrl: "public/Simon.png",
            MaxHealth: 50,
            CurrentHealth: 50,
            Attack: 5,
            Defense: 9,
            TechAttack: 10,
            TechDefense: 8,
            AbilityIds: new[] { 37, 38, 39 ,40 }
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
