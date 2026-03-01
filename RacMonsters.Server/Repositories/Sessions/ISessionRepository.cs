using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Sessions
{
    public interface ISessionRepository
    {
        Task<Session> GetSession(int sessionId);
        Task<Session> CreateSession(Session createSession);
        Task<Session> UpdateSession(Session session);
    }
}
