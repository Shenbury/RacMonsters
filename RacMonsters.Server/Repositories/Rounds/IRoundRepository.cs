using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Rounds
{
    public interface IRoundRepository
    {
        Task<Round[]> GetRounds(int[] roundIds);
        Task<Round> ExecuteRound(Round executeRound);
    }
}
