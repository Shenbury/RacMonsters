using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Actions;

namespace RacMonsters.Server.Repositories.Rounds
{
    public class RoundRepository : IRoundRepository
    {
        public async Task<Round[]> GetRounds(int[] roundIds)
        {
            throw new NotImplementedException();
        }

        public async Task<Round> ExecuteRound(Round executeRound)
        {
            throw new NotImplementedException();
        }
    }
}
