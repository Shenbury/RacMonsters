using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Abilities
{
    /// <summary>
    /// Configures status effects for abilities
    /// This allows abilities from the database to have status effects configured in code
    /// </summary>
    public static class AbilityStatusEffectConfigurator
    {
        /// <summary>
        /// Configure status effects for an ability
        /// </summary>
        public static void ConfigureStatusEffects(Ability ability)
        {
            // Clear any existing status effects
            ability.StatusEffects.Clear();

            // Configure based on ability ID or name
            switch (ability.Id)
            {
                // FLAME STRIKE - Basic burn attack
                case 69:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Burn,
                        Duration = 3,
                        Power = 5,
                        ApplyChance = 0.5,
                        ApplyToSelf = false
                    });
                    break;

                // INFERNO BLAST - High damage with guaranteed burn
                case 70:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Burn,
                        Duration = 4,
                        Power = 7,
                        ApplyChance = 1.0,
                        ApplyToSelf = false
                    });
                    break;

                // TOXIC STRIKE - Poison + heal block
                case 71:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Poison,
                        Duration = 3,
                        Power = 4,
                        ApplyChance = 0.7,
                        ApplyToSelf = false
                    });
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.HealBlock,
                        Duration = 2,
                        ApplyChance = 0.5,
                        ApplyToSelf = false
                    });
                    break;

                // VENOM SPRAY - Strong poison
                case 72:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Poison,
                        Duration = 4,
                        Power = 6,
                        ApplyChance = 0.8,
                        ApplyToSelf = false
                    });
                    break;

                // POWER UP - Attack buff
                case 73:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.AttackUp,
                        Duration = 3,
                        Modifier = 0.5,  // 50% attack increase
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // IRON DEFENSE - Defense buff
                case 74:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.DefenseUp,
                        Duration = 3,
                        Modifier = 0.5,  // 50% defense increase
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // FOCUS ENERGY - Accuracy buff
                case 75:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.AccuracyUp,
                        Duration = 2,
                        Modifier = 0.3,  // 30% accuracy increase
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // INTIMIDATE - Attack debuff
                case 76:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.AttackDown,
                        Duration = 3,
                        Modifier = 0.3,  // 30% attack decrease
                        ApplyChance = 0.85,
                        ApplyToSelf = false
                    });
                    break;

                // ARMOR BREAK - Defense debuff
                case 77:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.DefenseDown,
                        Duration = 3,
                        Modifier = 0.4,  // 40% defense decrease
                        ApplyChance = 0.75,
                        ApplyToSelf = false
                    });
                    break;

                // BLIND - Accuracy debuff
                case 78:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.AccuracyDown,
                        Duration = 2,
                        Modifier = 0.4,  // 40% accuracy decrease
                        ApplyChance = 0.9,
                        ApplyToSelf = false
                    });
                    break;

                // SOLAR BEAM - Charging move
                case 79:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Charging,
                        Duration = 1,
                        RequiresCharging = true,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // SKULL BASH - Charging move
                case 80:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Charging,
                        Duration = 1,
                        RequiresCharging = true,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // HYPER BEAM - Ultimate charging move
                case 81:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Charging,
                        Duration = 1,
                        RequiresCharging = true,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // DRAGON RAGE - Burn + defense down
                case 82:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Burn,
                        Duration = 3,
                        Power = 5,
                        ApplyChance = 0.6,
                        ApplyToSelf = false
                    });
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.DefenseDown,
                        Duration = 2,
                        Modifier = 0.3,
                        ApplyChance = 0.5,
                        ApplyToSelf = false
                    });
                    break;

                // CURSED SLASH - Bleed + attack down
                case 83:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Bleed,
                        Duration = 3,
                        Power = 6,
                        ApplyChance = 0.7,
                        ApplyToSelf = false
                    });
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.AttackDown,
                        Duration = 2,
                        Modifier = 0.25,
                        ApplyChance = 0.6,
                        ApplyToSelf = false
                    });
                    break;

                // WAR CRY - Attack + defense up
                case 84:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.AttackUp,
                        Duration = 3,
                        Modifier = 0.4,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.DefenseUp,
                        Duration = 3,
                        Modifier = 0.3,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // SMOKE SCREEN - Evasion up
                case 85:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.EvasionUp,
                        Duration = 2,
                        Modifier = 0.2,  // 20% evasion increase
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // PROTECT - Protection from damage
                case 86:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.Protected,
                        Duration = 1,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // RECOVER PLUS - Heal + defense up
                case 87:
                    ability.StatusEffects.Add(new StatusEffectApplication
                    {
                        Type = StatusEffectType.DefenseUp,
                        Duration = 2,
                        Modifier = 0.3,
                        ApplyChance = 1.0,
                        ApplyToSelf = true
                    });
                    break;

                // Add more ability configurations here...
            }
        }

        /// <summary>
        /// Configure status effects for multiple abilities
        /// </summary>
        public static void ConfigureStatusEffects(IEnumerable<Ability> abilities)
        {
            foreach (var ability in abilities)
            {
                ConfigureStatusEffects(ability);
            }
        }
    }
}
