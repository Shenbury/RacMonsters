using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.AIs
{
    public class AIService : IAIService
    {
        private readonly Random _rand = new();

        public async Task<Ability> ChooseAbilityAsync(Character character)
        {
            if (character is null) throw new ArgumentNullException(nameof(character));

            var abilities = character.Abilities ?? Array.Empty<Ability>();
            if (abilities.Length == 0)
            {
                throw new InvalidOperationException("Character has no abilities to choose from.");
            }

            var idx = _rand.Next(abilities.Length);
            return await Task.FromResult(abilities[idx]);
        }
    }
}
