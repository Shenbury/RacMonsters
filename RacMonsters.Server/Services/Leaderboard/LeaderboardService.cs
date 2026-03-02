using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Leaderboard;

namespace RacMonsters.Server.Services.Leaderboard
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _repo;
        public LeaderboardService(ILeaderboardRepository repo) => _repo = repo;

        public async Task<LeaderboardEntry> AddEntry(LeaderboardEntry entry)
        {
            entry.Timestamp = DateTime.UtcNow;
            // ensure timestamp and pass through
            return await _repo.Create(entry);
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetTop(int limit) => await _repo.GetTop(limit);

        public async Task<LeaderboardEntry> Upsert(string name, int delta, string? character = null) => await _repo.Upsert(name, delta, character);
    }
}
