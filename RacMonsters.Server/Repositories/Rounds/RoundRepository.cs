using RacMonsters.Server.Models;

namespace RacMonsters.Server.Repositories.Rounds
{
    public class RoundRepository : IRoundRepository
    {
        public async Task<Round[]> GetRounds(int[] roundIds)
        {
            // this simple implementation does not persist rounds, return empty
            return await Task.FromResult(Array.Empty<Round>());
        }

        public async Task<Round> ExecuteRound(Round executeRound)
        {
            // very small deterministic resolution: higher speed goes first
            var a = executeRound.PlayerA.Character;
            var b = executeRound.PlayerB.Character;

            var moveA = executeRound.PlayerA.Ability;
            var moveB = executeRound.PlayerB.Ability;

            void ApplyMove(Character attacker, Character defender, Ability ability)
            {
                if (ability.IsHeal)
                {
                    var heal = ability.Power;
                    attacker.CurrentHealth = Math.Min(attacker.MaxHealth, attacker.CurrentHealth + heal);
                }
                else
                {
                    var attackStat = ability.IsTech ? attacker.TechAttack : attacker.Attack;
                    var defenseStat = ability.IsTech ? defender.TechDefense : defender.Defense;
                    var damage = Math.Max(1, ability.Power + attackStat - defenseStat);
                    defender.CurrentHealth = Math.Max(0, defender.CurrentHealth - damage);
                }
            }

            if (moveA.Speed >= moveB.Speed)
            {
                ApplyMove(a, b, moveA);
                if (b.CurrentHealth > 0) ApplyMove(b, a, moveB);
            }
            else
            {
                ApplyMove(b, a, moveB);
                if (a.CurrentHealth > 0) ApplyMove(a, b, moveA);
            }

            return await Task.FromResult(executeRound);
        }
    }
}
