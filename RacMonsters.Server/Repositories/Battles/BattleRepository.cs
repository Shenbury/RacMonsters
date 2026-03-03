using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Battles
{
    public class BattleRepository : IBattleRepository
    {
        private static readonly List<Battle> _battles = new();
        private static int _nextId = 1;

        public async Task<Battle> CreateBattle(Battle createBattle)
        {
            createBattle.Id = _nextId++;
            _battles.Add(createBattle);
            return await Task.FromResult(createBattle);
        }

        public async Task<Battle> GetBattle(int battleId)
        {
            var b = _battles.FirstOrDefault(x => x.Id == battleId);
            return await Task.FromResult(b!);
        }

        public async Task<Battle> UpdateBattle(Battle battle)
        {
            var existing = _battles.FirstOrDefault(x => x.Id == battle.Id);
            if (existing != null)
            {
                var index = _battles.IndexOf(existing);
                _battles[index] = battle;
            }
            return await Task.FromResult(battle);
        }

        public async Task<List<Battle>> GetBattlesWithExpiredTurns()
        {
            var now = DateTime.UtcNow;
            var expiredBattles = _battles.Where(b => 
                b.IsMultiplayer && 
                b.WinningCharacter == null && 
                b.TurnStartTime.HasValue &&
                (now - b.TurnStartTime.Value).TotalSeconds > b.TurnTimeoutSeconds
            ).ToList();

            return await Task.FromResult(expiredBattles);
        }
    }
}
