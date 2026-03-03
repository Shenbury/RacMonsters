namespace RacMonsters.Server.Services.Matchmaking
{
    /// <summary>
    /// Temporary stub implementation of IMatchmakingService.
    /// This will be replaced with full implementation in Phase 3.
    /// </summary>
    public class MatchmakingServiceStub : IMatchmakingService
    {
        private readonly ILogger<MatchmakingServiceStub> _logger;

        public MatchmakingServiceStub(ILogger<MatchmakingServiceStub> logger)
        {
            _logger = logger;
        }

        public Task AddPlayerToQueue(string connectionId, string playerName, int characterId)
        {
            _logger.LogInformation($"[STUB] Adding player {playerName} to queue - Will be implemented in Phase 3");
            return Task.CompletedTask;
        }

        public Task RemovePlayer(string connectionId)
        {
            _logger.LogInformation($"[STUB] Removing player {connectionId} from queue - Will be implemented in Phase 3");
            return Task.CompletedTask;
        }

        public Task<int> GetQueueSize()
        {
            _logger.LogInformation("[STUB] Getting queue size - Will be implemented in Phase 3");
            return Task.FromResult(0);
        }
    }
}
