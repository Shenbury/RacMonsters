using RacMonsters.Server.Repositories.Characters;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Characters
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }
        
        public Character[] GetAll()
        {
            return _characterRepository.GetAll().GetAwaiter().GetResult();
        }

        public async Task<Character?> GetById(int id)
        {
            return await _characterRepository.GetCharacter(id);
        }

        public async Task<Character> GetRandomCharacter()
        {
            return await _characterRepository.GetRandomCharacter();
        }
    }
}
