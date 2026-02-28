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
    }
}
