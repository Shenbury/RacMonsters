using RacMonsters.Server.Repositories.Sessions;
using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Battles;

namespace RacMonsters.Server.Services.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IBattleRepository _battleRepository;

        public SessionService(ISessionRepository sessionRepository, IBattleRepository battleRepository)
        {
            _sessionRepository = sessionRepository;
            _battleRepository = battleRepository;
        }

        public async Task<Session> CreateSession(Session session)
        {
            session.Battles ??= Array.Empty<Battle>();
            return await _sessionRepository.CreateSession(session);
        }

        public async Task<Session?> GetSession(int id)
        {
            return await _sessionRepository.GetSession(id);
        }
    }
}
