using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Battles
{
    /// <summary>
    /// Shared logic for processing status effects in battles
    /// Used by both multiplayer and single-player battle systems
    /// </summary>
    public static class StatusEffectProcessor
    {
        public static void ProcessStartOfTurnEffects(Character character, List<string> messages)
        {
            // Process Damage over Time effects
            var dotEffects = character.ActiveStatusEffects
                .Where(e => e.Type == StatusEffectType.Burn || 
                           e.Type == StatusEffectType.Poison || 
                           e.Type == StatusEffectType.Bleed)
                .ToList();

            foreach (var effect in dotEffects)
            {
                var damage = effect.Power;
                character.CurrentHealth = Math.Max(0, character.CurrentHealth - damage);
                messages.Add($"{character.Name} took {damage} damage from {effect.GetDisplayName()}!");
                
                if (character.CurrentHealth <= 0)
                {
                    messages.Add($"{character.Name} was defeated by {effect.GetDisplayName()}!");
                    break;
                }
            }
        }

        public static void ProcessEndOfTurnEffects(Character character, List<string> messages)
        {
            // Decrement all status effect durations
            foreach (var effect in character.ActiveStatusEffects.ToList())
            {
                effect.DecrementDuration();
                
                if (effect.IsExpired)
                {
                    character.ActiveStatusEffects.Remove(effect);
                    messages.Add($"{character.Name}'s {effect.GetDisplayName()} wore off!");
                }
            }
        }

        public static (int damage, int heal, bool hit, List<string> statusMessages) ProcessAbilityWithStatusEffects(
            Character attacker, 
            Ability ability, 
            Character defender)
        {
            var statusMessages = new List<string>();
            var chargingEffect = attacker.GetChargingEffect();
            
            // Check if this is completing a charging move
            if (chargingEffect != null && chargingEffect.ChargingAbilityId.HasValue)
            {
                var chargedAbility = attacker.Abilities.FirstOrDefault(a => a.Id == chargingEffect.ChargingAbilityId.Value);
                if (chargedAbility != null)
                {
                    ability = chargedAbility;
                    statusMessages.Add($"{attacker.Name} unleashes {ability.Name}!");
                    attacker.ActiveStatusEffects.Remove(chargingEffect);
                }
            }
            // Check if this ability requires charging
            else if (ability.IsChargingMove)
            {
                // Start charging
                var chargingStatusEffect = new StatusEffect
                {
                    Type = StatusEffectType.Charging,
                    Name = "Charging",
                    Description = $"Charging {ability.Name}",
                    Duration = 1,
                    Power = 0,
                    ChargingAbilityId = ability.Id,
                    SourceAbilityName = ability.Name
                };
                attacker.ActiveStatusEffects.Add(chargingStatusEffect);
                statusMessages.Add($"{attacker.Name} begins charging {ability.Name}!");
                
                return (0, 0, true, statusMessages);
            }

            // Check if attacker is stunned
            if (attacker.IsStunned())
            {
                statusMessages.Add($"{attacker.Name} is stunned and cannot act!");
                return (0, 0, false, statusMessages);
            }

            // Process the ability normally
            var (damage, heal, hit) = ProcessAbility(attacker, ability, defender);

            // Add message if heal was blocked
            if (ability.IsHeal && !hit && attacker.HasHealBlock())
            {
                statusMessages.Add($"{attacker.Name} tried to use {ability.Name} but healing is blocked!");
            }

            // Apply status effects if the ability hit
            if (hit && ability.AppliesStatusEffects)
            {
                var random = new Random();
                foreach (var statusApp in ability.StatusEffects)
                {
                    // Check if status effect should be applied based on chance
                    if (random.NextDouble() <= statusApp.ApplyChance)
                    {
                        var target = statusApp.ApplyToSelf ? attacker : defender;
                        ApplyStatusEffect(target, statusApp, ability.Name, statusMessages);
                    }
                }
            }

            return (damage, heal, hit, statusMessages);
        }

        public static (int damage, int heal, bool hit) ProcessAbility(Character attacker, Ability ability, Character defender)
        {
            var random = new Random();
            
            // Calculate accuracy with modifiers
            var finalAccuracy = ability.Accuracy * attacker.GetAccuracyModifier();
            var evasionMod = defender.GetEvasionModifier();
            finalAccuracy = Math.Max(0.1, Math.Min(1.0, finalAccuracy - evasionMod));
            
            var hitSuccess = random.NextDouble() < finalAccuracy;

            if (!hitSuccess)
            {
                return (0, 0, false);
            }

            if (ability.IsHeal)
            {
                // Check for heal block
                if (attacker.HasHealBlock())
                {
                    return (0, 0, false);
                }
                
                int healAmount = ability.Power;
                attacker.CurrentHealth = Math.Min(
                    attacker.CurrentHealth + healAmount,
                    attacker.MaxHealth
                );
                return (0, healAmount, true);
            }
            else
            {
                // Check if defender is protected
                if (defender.IsProtected())
                {
                    return (0, 0, false);
                }
                
                // Calculate damage based on ability type with status modifiers
                var attackStat = ability.IsTech ? attacker.GetEffectiveTechAttack() : attacker.GetEffectiveAttack();
                var defenseStat = ability.IsTech ? defender.GetEffectiveTechDefense() : defender.GetEffectiveDefense();

                int damage = Math.Max(1, (ability.Power + attackStat) - defenseStat);
                defender.CurrentHealth = Math.Max(0, defender.CurrentHealth - damage);

                return (damage, 0, true);
            }
        }

        public static void ApplyStatusEffect(Character target, StatusEffectApplication statusApp, string sourceName, List<string> messages)
        {
            var statusEffect = new StatusEffect
            {
                Type = statusApp.Type,
                Name = GetStatusEffectName(statusApp.Type),
                Description = GetStatusEffectDescription(statusApp.Type),
                Duration = statusApp.Duration,
                Power = statusApp.Power,
                Modifier = statusApp.Modifier,
                SourceAbilityName = sourceName
            };

            // Check if the status effect already exists and refresh/stack it
            var existing = target.ActiveStatusEffects.FirstOrDefault(e => e.Type == statusApp.Type);
            if (existing != null)
            {
                // Refresh duration and update power if new one is stronger
                existing.Duration = Math.Max(existing.Duration, statusEffect.Duration);
                existing.Power = Math.Max(existing.Power, statusEffect.Power);
                messages.Add($"{target.Name}'s {statusEffect.GetDisplayName()} was refreshed!");
            }
            else
            {
                target.ActiveStatusEffects.Add(statusEffect);
                messages.Add($"{target.Name} is now affected by {statusEffect.GetDisplayName()}!");
            }
        }

        public static string FormatAbilityResult(Character attacker, Ability ability, Character defender, int damage, int heal, bool hit)
        {
            if (!hit)
            {
                return $"{attacker.Name} tried to use {ability.Name} but it missed!";
            }

            if (ability.IsHeal)
            {
                if (attacker.HasHealBlock())
                {
                    return $"{attacker.Name} tried to use {ability.Name} but healing is blocked!";
                }
                return $"{attacker.Name} used {ability.Name} and healed for {heal} HP!";
            }
            else
            {
                if (defender.IsProtected())
                {
                    return $"{attacker.Name} used {ability.Name} but {defender.Name} is protected!";
                }
                return $"{attacker.Name} used {ability.Name} on {defender.Name} for {damage} damage!";
            }
        }

        private static string GetStatusEffectName(StatusEffectType type)
        {
            return type switch
            {
                StatusEffectType.Burn => "Burn",
                StatusEffectType.Poison => "Poison",
                StatusEffectType.Bleed => "Bleed",
                StatusEffectType.AttackUp => "Attack Up",
                StatusEffectType.AttackDown => "Attack Down",
                StatusEffectType.DefenseUp => "Defense Up",
                StatusEffectType.DefenseDown => "Defense Down",
                StatusEffectType.TechAttackUp => "Tech Attack Up",
                StatusEffectType.TechAttackDown => "Tech Attack Down",
                StatusEffectType.TechDefenseUp => "Tech Defense Up",
                StatusEffectType.TechDefenseDown => "Tech Defense Down",
                StatusEffectType.AccuracyUp => "Accuracy Up",
                StatusEffectType.AccuracyDown => "Accuracy Down",
                StatusEffectType.EvasionUp => "Evasion Up",
                StatusEffectType.EvasionDown => "Evasion Down",
                StatusEffectType.Charging => "Charging",
                StatusEffectType.HealBlock => "Heal Block",
                StatusEffectType.Stunned => "Stunned",
                StatusEffectType.Protected => "Protected",
                _ => "Unknown"
            };
        }

        private static string GetStatusEffectDescription(StatusEffectType type)
        {
            return type switch
            {
                StatusEffectType.Burn => "Takes damage each turn",
                StatusEffectType.Poison => "Takes increasing damage each turn",
                StatusEffectType.Bleed => "Takes damage each turn",
                StatusEffectType.AttackUp => "Increased physical attack",
                StatusEffectType.AttackDown => "Decreased physical attack",
                StatusEffectType.DefenseUp => "Increased physical defense",
                StatusEffectType.DefenseDown => "Decreased physical defense",
                StatusEffectType.TechAttackUp => "Increased tech attack",
                StatusEffectType.TechAttackDown => "Decreased tech attack",
                StatusEffectType.TechDefenseUp => "Increased tech defense",
                StatusEffectType.TechDefenseDown => "Decreased tech defense",
                StatusEffectType.AccuracyUp => "Increased hit chance",
                StatusEffectType.AccuracyDown => "Decreased hit chance",
                StatusEffectType.EvasionUp => "Increased dodge chance",
                StatusEffectType.EvasionDown => "Decreased dodge chance",
                StatusEffectType.Charging => "Preparing a powerful attack",
                StatusEffectType.HealBlock => "Cannot heal",
                StatusEffectType.Stunned => "Cannot act",
                StatusEffectType.Protected => "Protected from damage",
                _ => "Unknown effect"
            };
        }
    }
}
