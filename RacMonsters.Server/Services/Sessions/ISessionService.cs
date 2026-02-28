using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Sessions
{
    public interface ISessionService
    {
        Task<Session> CreateSession(Session session);
        Task<Session?> GetSession(int id);
    }
}
