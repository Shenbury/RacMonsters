namespace RacMonsters.Server.Models
{
    public enum StatusEffectType
    {
        // Damage over time
        Burn,
        Poison,
        Bleed,
        
        // Stat modifiers
        AttackUp,
        AttackDown,
        DefenseUp,
        DefenseDown,
        TechAttackUp,
        TechAttackDown,
        TechDefenseUp,
        TechDefenseDown,
        AccuracyUp,
        AccuracyDown,
        EvasionUp,
        EvasionDown,
        
        // Special effects
        Charging,
        HealBlock,
        Stunned,
        Protected
    }
}
