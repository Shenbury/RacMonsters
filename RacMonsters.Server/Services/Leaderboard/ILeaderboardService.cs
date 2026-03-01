using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Leaderboard
{
    public interface ILeaderboardService
    {
        Task<LeaderboardEntry> AddEntry(LeaderboardEntry entry);
        Task<IEnumerable<LeaderboardEntry>> GetTop(int limit);
        Task<LeaderboardEntry> Upsert(string name, int delta);
    }
}
