namespace RacMonsters.Server.Models
{
    public class StatusEffect
    {
        public StatusEffectType Type { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Duration { get; set; }
        public int Power { get; set; }  // For DoTs or stat modification amount
        public double Modifier { get; set; } = 1.0;  // Multiplier for stat changes
        public string? SourceAbilityName { get; set; }  // Name of ability that applied this
        public int? ChargingAbilityId { get; set; }  // For charging moves, stores the ability to execute
        
        public bool IsExpired => Duration <= 0;
        
        public void DecrementDuration()
        {
            if (Duration > 0)
            {
                Duration--;
            }
        }
        
        public string GetDisplayName()
        {
            return Type switch
            {
                StatusEffectType.Burn => "🔥 Burn",
                StatusEffectType.Poison => "☠️ Poison",
                StatusEffectType.Bleed => "🩸 Bleed",
                StatusEffectType.AttackUp => "⚔️↑ Attack Up",
                StatusEffectType.AttackDown => "⚔️↓ Attack Down",
                StatusEffectType.DefenseUp => "🛡️↑ Defense Up",
                StatusEffectType.DefenseDown => "🛡️↓ Defense Down",
                StatusEffectType.TechAttackUp => "⚡↑ Tech Attack Up",
                StatusEffectType.TechAttackDown => "⚡↓ Tech Attack Down",
                StatusEffectType.TechDefenseUp => "🔰↑ Tech Defense Up",
                StatusEffectType.TechDefenseDown => "🔰↓ Tech Defense Down",
                StatusEffectType.AccuracyUp => "🎯↑ Accuracy Up",
                StatusEffectType.AccuracyDown => "🎯↓ Accuracy Down",
                StatusEffectType.EvasionUp => "💨↑ Evasion Up",
                StatusEffectType.EvasionDown => "💨↓ Evasion Down",
                StatusEffectType.Charging => "⚡ Charging",
                StatusEffectType.HealBlock => "🚫 Heal Block",
                StatusEffectType.Stunned => "💫 Stunned",
                StatusEffectType.Protected => "✨ Protected",
                _ => Name
            };
        }
    }
}
