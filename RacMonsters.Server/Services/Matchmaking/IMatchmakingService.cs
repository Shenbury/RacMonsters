using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Matchmaking
{
    public interface IMatchmakingService
    {
        Task AddPlayerToQueue(string connectionId, string playerName, int characterId);
        Task RemovePlayer(string connectionId);
        Task<int> GetQueueSize();
    }
}
