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
                1,
                "Faisal",
                "public/Faisal.png",
                30,
                30,
                6,
                4,
                5,
                3,
                new[] { 1, 3 }
            ),
            new CharacterTemplate(
                2,
                "Simon",
                "public/Simon.png",
                32,
                32,
                5,
                5,
                6,
                4,
                new[] { 2, 1 }
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
