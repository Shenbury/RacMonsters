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

            void ApplyMove(Character attacker, Character defender, Ability ability, RoundAction action)
            {
                // Check accuracy: ability may miss and have no effect
                var hitRoll = System.Random.Shared.NextDouble();
                if (ability.Accuracy < 1.0 && hitRoll > ability.Accuracy)
                {
                    // missed — set flag and message
                    action.Hit = false;
                    action.ResultMessage = $"{attacker.Name} missed!";
                    return;
                }

                // Determine relevant stats
                var attackStat = ability.IsTech ? attacker.TechAttack : attacker.Attack;
                var defenseStat = ability.IsTech ? defender.TechDefense : defender.Defense;

                action.Hit = true;

                if (ability.IsHeal)
                {
                    // Heal amount is random between 1 and (Power + attackStat), minimum 1
                    var maxHeal = Math.Max(1, ability.Power + attackStat);
                    var healAmount = System.Random.Shared.Next(1, maxHeal + 1);
                    attacker.CurrentHealth = Math.Min(attacker.MaxHealth, attacker.CurrentHealth + healAmount);
                    action.HealAmount = healAmount;
                    action.ResultMessage = $"{attacker.Name} healed {healAmount} HP.";
                }
                else
                {
                    // Damage = (random 1..Power+attackStat) - (random 1..defenseStat)
                    var maxAttack = Math.Max(1, ability.Power + attackStat);
                    var attackRoll = System.Random.Shared.Next(1, maxAttack + 1);

                    int defenseRoll;
                    if (defenseStat <= 0)
                    {
                        defenseRoll = 0;
                    }
                    else
                    {
                        defenseRoll = System.Random.Shared.Next(1, defenseStat + 1);
                    }

                    var rawDamage = attackRoll - defenseRoll;
                    var damage = Math.Max(1, rawDamage);
                    defender.CurrentHealth = Math.Max(0, defender.CurrentHealth - damage);
                    action.Damage = damage;
                    action.ResultMessage = $"{attacker.Name} dealt {damage} damage to {defender.Name}.";
                }
            }

            if (moveA.Speed >= moveB.Speed)
            {
                ApplyMove(a, b, moveA, executeRound.PlayerA);
                if (b.CurrentHealth > 0) ApplyMove(b, a, moveB, executeRound.PlayerB);
            }
            else
            {
                ApplyMove(b, a, moveB, executeRound.PlayerB);
                if (a.CurrentHealth > 0) ApplyMove(a, b, moveA, executeRound.PlayerA);
            }

            return await Task.FromResult(executeRound);
        }
    }
}
