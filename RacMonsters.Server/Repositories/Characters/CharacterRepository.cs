using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Characters
{
    public class CharacterRepository : ICharacterRepository
    {
        private static readonly Character[] _characters = new[]
        {
            new Character
            {
                Id = 1,
                Name = "Racmon A",
                ImageUrl = string.Empty,
                MaxHealth = 30,
                CurrentHealth = 30,
                Attack = 6,
                Defense = 4,
                TechAttack = 5,
                TechDefense = 3,
                Abilities = new[]
                {
                    new Ability { Id = 1, Name = "Tackle", Description = "A basic physical attack.", IsTech = false, Power = 5, Speed = 5, Accuracy = 0.95 },
                    new Ability { Id = 3, Name = "Heal", Description = "Restore some health.", IsTech = true, IsHeal = true, Power = 8, Speed = 4, Accuracy = 1.0 }
                }
            },
            new Character
            {
                Id = 2,
                Name = "Racmon B",
                ImageUrl = string.Empty,
                MaxHealth = 32,
                CurrentHealth = 32,
                Attack = 5,
                Defense = 5,
                TechAttack = 6,
                TechDefense = 4,
                Abilities = new[]
                {
                    new Ability { Id = 2, Name = "Ember", Description = "A basic tech fire attack.", IsTech = true, Power = 7, Speed = 6, Accuracy = 0.9 },
                    new Ability { Id = 1, Name = "Tackle", Description = "A basic physical attack.", IsTech = false, Power = 5, Speed = 5, Accuracy = 0.95 }
                }
            }
        };

        public async Task<Character> GetCharacter(int id)
        {
            var ch = _characters.FirstOrDefault(c => c.Id == id);
            return await Task.FromResult(ch!);
        }

        public async Task<Character> GetRandomCharacter()
        {
            var r = new Random();
            var ch = _characters[r.Next(_characters.Length)];
            return await Task.FromResult(ch);
        }

        public async Task<Character[]> GetAll()
        {
            return await Task.FromResult(_characters);
        }
    }
}
