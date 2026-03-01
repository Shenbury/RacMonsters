using Microsoft.EntityFrameworkCore;
using RacMonsters.Server.Data;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Leaderboard
{
    public class SqlLeaderboardRepository : ILeaderboardRepository
    {
        private readonly RacMonstersDbContext _db;
        public SqlLeaderboardRepository(RacMonstersDbContext db) => _db = db;

        public async Task<LeaderboardEntry> Create(LeaderboardEntry entry)
        {
            var entity = new LeaderboardEntity
            {
                Name = entry.Name,
                Score = entry.Score,
                Timestamp = entry.Timestamp
            };
            _db.Leaderboard.Add(entity);
            await _db.SaveChangesAsync();
            entry.Id = entity.Id;
            return entry;
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetTop(int limit)
        {
            return await _db.Leaderboard.OrderByDescending(l => l.Score).ThenBy(l => l.Timestamp).Take(limit)
                .Select(e => new LeaderboardEntry { Id = e.Id, Name = e.Name, Score = e.Score, Timestamp = e.Timestamp })
                .ToListAsync();
        }

        public async Task<LeaderboardEntry> Upsert(string name, int delta)
        {
            var entity = await _db.Leaderboard.FirstOrDefaultAsync(l => l.Name == name);
            if (entity == null)
            {
                entity = new LeaderboardEntity { Name = name, Score = delta, Timestamp = DateTime.UtcNow };
                _db.Leaderboard.Add(entity);
            }
            else
            {
                entity.Score += delta;
                entity.Timestamp = DateTime.UtcNow;
                _db.Leaderboard.Update(entity);
            }

            await _db.SaveChangesAsync();
            return new LeaderboardEntry { Id = entity.Id, Name = entity.Name, Score = entity.Score, Timestamp = entity.Timestamp };
        }
    }
}
