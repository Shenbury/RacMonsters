using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Rounds;

namespace RacMonsters.Server.Services.Rounds
{
    public class RoundService : IRoundService
    {
        private readonly IRoundRepository _roundRepository;

        public RoundService(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository;
        }
        
        public async Task<Round> ExecuteRound(Round r)
        {
            return await _roundRepository.ExecuteRound(r);
        }
    }
}
