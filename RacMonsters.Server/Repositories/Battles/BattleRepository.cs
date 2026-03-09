using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Battles
{
    public class BattleRepository : IBattleRepository
    {
        private static readonly List<Battle> _battles = new();
        private static int _nextId = 1;
        private static readonly object _lock = new();
        private const int MaxCompletedBattlesToKeep = 100; // Keep last 100 completed battles

        public async Task<Battle> CreateBattle(Battle createBattle)
        {
            lock (_lock)
            {
                createBattle.Id = _nextId++;
                _battles.Add(createBattle);

                // Clean up old completed battles to prevent memory leak
                CleanupCompletedBattles();
            }
            return await Task.FromResult(createBattle);
        }

        public async Task<Battle> GetBattle(int battleId)
        {
            Battle? b;
            lock (_lock)
            {
                b = _battles.FirstOrDefault(x => x.Id == battleId);
            }
            return await Task.FromResult(b!);
        }

        public async Task<Battle> UpdateBattle(Battle battle)
        {
            lock (_lock)
            {
                var existing = _battles.FirstOrDefault(x => x.Id == battle.Id);
                if (existing != null)
                {
                    var index = _battles.IndexOf(existing);
                    _battles[index] = battle;
                }
            }
            return await Task.FromResult(battle);
        }

        public async Task<List<Battle>> GetBattlesWithExpiredTurns()
        {
            var now = DateTime.UtcNow;
            List<Battle> expiredBattles;

            lock (_lock)
            {
                expiredBattles = _battles.Where(b => 
                    b.IsMultiplayer && 
                    b.WinningCharacter == null && 
                    b.TurnStartTime.HasValue &&
                    (now - b.TurnStartTime.Value).TotalSeconds > b.TurnTimeoutSeconds
                ).ToList();
            }

            return await Task.FromResult(expiredBattles);
        }

        private void CleanupCompletedBattles()
        {
            // Must be called within lock
            var completedBattles = _battles
                .Where(b => b.WinningCharacter != null || b.IsMultiplayer && !string.IsNullOrEmpty(b.Player1ConnectionId))
                .OrderByDescending(b => b.Id)
                .Skip(MaxCompletedBattlesToKeep)
                .ToList();

            if (completedBattles.Count > 0)
            {
                foreach (var battle in completedBattles)
                {
                    _battles.Remove(battle);
                }
            }
        }
    }
}
