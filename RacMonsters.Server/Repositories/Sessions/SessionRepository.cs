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

        public async Task<Session> UpdateSession(Session session)
        {
            var idx = _sessions.FindIndex(x => x.Id == session.Id);
            if (idx >= 0)
            {
                _sessions[idx] = session;
            }
            else
            {
                // if session not present, add it
                _sessions.Add(session);
            }

            return await Task.FromResult(session);
        }
    }
}
