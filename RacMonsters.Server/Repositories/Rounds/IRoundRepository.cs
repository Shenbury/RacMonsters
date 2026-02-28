using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Actions
{
    public interface IRoundRepository
    {
        Task<Round[]> GetRounds(int[] roundIds);
        public Task<Round> ExecuteRound(Round executeRound);
    }
}
