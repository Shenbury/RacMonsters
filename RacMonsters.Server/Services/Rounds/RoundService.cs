using RacMonsters.Server.Repositories.Actions;

namespace RacMonsters.Server.Services.Rounds
{
    public class RoundService : IRoundService
    {
        private readonly IRoundRepository _roundRepository;

        public RoundService(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository;
        }
    }
}
