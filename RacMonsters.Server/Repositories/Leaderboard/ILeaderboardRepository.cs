using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Leaderboard
{
    public interface ILeaderboardRepository
    {
        Task<LeaderboardEntry> Create(LeaderboardEntry entry);
        Task<IEnumerable<LeaderboardEntry>> GetTop(int limit);
        Task<LeaderboardEntry> Upsert(string name, int delta, string? character = null);
    }
}
