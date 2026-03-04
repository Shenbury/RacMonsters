using RacMonsters.Server.Models;
using RacMonsters.Server.Services.Battles;

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
            var a = executeRound.PlayerA.Character;
            var b = executeRound.PlayerB.Character;

            var moveA = executeRound.PlayerA.Ability;
            var moveB = executeRound.PlayerB.Ability;

            var resultMessages = new List<string>();
            var statusMessages = new List<string>();

            // Process start-of-turn status effects (DoTs, etc.)
            StatusEffectProcessor.ProcessStartOfTurnEffects(a, resultMessages);
            StatusEffectProcessor.ProcessStartOfTurnEffects(b, resultMessages);

            // Check if either character is defeated by DoT
            if (a.CurrentHealth <= 0 || b.CurrentHealth <= 0)
            {
                // Battle ended due to DoT
                executeRound.PlayerA.ResultMessage = a.CurrentHealth <= 0 ? "Defeated by status effect" : string.Join(" | ", resultMessages);
                executeRound.PlayerB.ResultMessage = b.CurrentHealth <= 0 ? "Defeated by status effect" : string.Join(" | ", resultMessages);

                StatusEffectProcessor.ProcessEndOfTurnEffects(a, statusMessages);
                StatusEffectProcessor.ProcessEndOfTurnEffects(b, statusMessages);

                return await Task.FromResult(executeRound);
            }

            // Determine turn order based on speed
            bool aGoesFirst = moveA.Speed >= moveB.Speed;

            if (aGoesFirst)
            {
                // Character A goes first
                var resultA = StatusEffectProcessor.ProcessAbilityWithStatusEffects(a, moveA, b);
                executeRound.PlayerA.Hit = resultA.hit;
                executeRound.PlayerA.Damage = resultA.damage;
                executeRound.PlayerA.HealAmount = resultA.heal;
                executeRound.PlayerA.ResultMessage = StatusEffectProcessor.FormatAbilityResult(a, moveA, b, resultA.damage, resultA.heal, resultA.hit);
                executeRound.PlayerA.StatusEffectMessages = resultA.statusMessages;
                resultMessages.Add(executeRound.PlayerA.ResultMessage);

                // Character B goes second (if still alive and not stunned)
                if (b.CurrentHealth > 0)
                {
                    var resultB = StatusEffectProcessor.ProcessAbilityWithStatusEffects(b, moveB, a);
                    executeRound.PlayerB.Hit = resultB.hit;
                    executeRound.PlayerB.Damage = resultB.damage;
                    executeRound.PlayerB.HealAmount = resultB.heal;
                    executeRound.PlayerB.ResultMessage = StatusEffectProcessor.FormatAbilityResult(b, moveB, a, resultB.damage, resultB.heal, resultB.hit);
                    executeRound.PlayerB.StatusEffectMessages = resultB.statusMessages;
                    resultMessages.Add(executeRound.PlayerB.ResultMessage);
                }
                else
                {
                    executeRound.PlayerB.Hit = false;
                    executeRound.PlayerB.ResultMessage = $"{b.Name} was defeated before acting";
                }
            }
            else
            {
                // Character B goes first
                var resultB = StatusEffectProcessor.ProcessAbilityWithStatusEffects(b, moveB, a);
                executeRound.PlayerB.Hit = resultB.hit;
                executeRound.PlayerB.Damage = resultB.damage;
                executeRound.PlayerB.HealAmount = resultB.heal;
                executeRound.PlayerB.ResultMessage = StatusEffectProcessor.FormatAbilityResult(b, moveB, a, resultB.damage, resultB.heal, resultB.hit);
                executeRound.PlayerB.StatusEffectMessages = resultB.statusMessages;
                resultMessages.Add(executeRound.PlayerB.ResultMessage);

                // Character A goes second (if still alive and not stunned)
                if (a.CurrentHealth > 0)
                {
                    var resultA = StatusEffectProcessor.ProcessAbilityWithStatusEffects(a, moveA, b);
                    executeRound.PlayerA.Hit = resultA.hit;
                    executeRound.PlayerA.Damage = resultA.damage;
                    executeRound.PlayerA.HealAmount = resultA.heal;
                    executeRound.PlayerA.ResultMessage = StatusEffectProcessor.FormatAbilityResult(a, moveA, b, resultA.damage, resultA.heal, resultA.hit);
                    executeRound.PlayerA.StatusEffectMessages = resultA.statusMessages;
                    resultMessages.Add(executeRound.PlayerA.ResultMessage);
                }
                else
                {
                    executeRound.PlayerA.Hit = false;
                    executeRound.PlayerA.ResultMessage = $"{a.Name} was defeated before acting";
                }
            }

            // Process end-of-turn effects (decrement durations, remove expired effects)
            StatusEffectProcessor.ProcessEndOfTurnEffects(a, statusMessages);
            StatusEffectProcessor.ProcessEndOfTurnEffects(b, statusMessages);

            return await Task.FromResult(executeRound);
        }
    }
}
