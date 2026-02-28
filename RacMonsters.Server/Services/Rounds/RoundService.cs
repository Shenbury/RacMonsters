using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Rounds;
using RacMonsters.Server.Services.AIs;

namespace RacMonsters.Server.Services.Rounds
{
    public class RoundService : IRoundService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IAIService _aiService;

        public RoundService(IRoundRepository roundRepository, IAIService aiService)
        {
            _roundRepository = roundRepository;
            _aiService = aiService;
        }
        
        public async Task<Round> ExecuteRound(Round r)
        {
            // Ensure both participants have chosen an ability. If missing, let AI choose.
            if (r.PlayerA == null || r.PlayerA.Character == null)
            {
                throw new ArgumentException("PlayerA and its Character must be provided on the round.");
            }

            if (r.PlayerB == null || r.PlayerB.Character == null)
            {
                throw new ArgumentException("PlayerB and its Character must be provided on the round.");
            }

            if (r.PlayerA.Ability == null)
            {
                r.PlayerA.Ability = await _aiService.ChooseAbilityAsync(r.PlayerA.Character);
            }

            if (r.PlayerB.Ability == null)
            {
                r.PlayerB.Ability = await _aiService.ChooseAbilityAsync(r.PlayerB.Character);
            }

            // Now both abilities are present — delegate to repository to execute.
            return await _roundRepository.ExecuteRound(r);
        }
    }
}
