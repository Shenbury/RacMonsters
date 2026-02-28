using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Characters
{
    public interface ICharacterRepository
    {
        Task<Character> GetRandomCharacter();
        Task<Character> GetCharacter(int id);
    }
}
