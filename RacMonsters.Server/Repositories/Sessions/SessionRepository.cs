using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Sessions
{
    public class SessionRepository : ISessionRepository
    {
        private static readonly List<Session> _sessions = new();
        private static int _nextId = 1;

        public async Task<Session> CreateSession(Session createSession)
        {
            createSession.Id = _nextId++;
            _sessions.Add(createSession);
            return await Task.FromResult(createSession);
        }

        public async Task<Session> GetSession(int sessionId)
        {
            var s = _sessions.FirstOrDefault(x => x.Id == sessionId);
            return await Task.FromResult(s!);
        }
    }
}
