using RacMonsters.Server.Repositories.Sessions;

namespace RacMonsters.Server.Services.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository) 
        {
            _sessionRepository = sessionRepository;
        }
    }
}
