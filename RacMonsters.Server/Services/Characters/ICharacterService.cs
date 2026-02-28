using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Characters
{
    public interface ICharacterService
    {
        Character[] GetAll();
        Task<Character?> GetById(int id);
        Task<Character> GetRandomCharacter();
    }
}
