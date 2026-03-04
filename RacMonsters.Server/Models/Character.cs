namespace RacMonsters.Server.Models;

public class Character
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = string.Empty;

    public int CurrentHealth { get; set; }
    public int MaxHealth { get; init; }

    // abilities available to this character
    public Ability[] Abilities { get; init; } = Array.Empty<Ability>();

    public int Attack { get; init; }
    public int Defense { get; init; }
    public int TechAttack { get; init; }
    public int TechDefense { get; init; }

    // Active status effects
    public List<StatusEffect> ActiveStatusEffects { get; set; } = new List<StatusEffect>();

    // Helper property for team battles
    public bool IsKnockedOut => CurrentHealth <= 0;

    // Calculate effective stats with status effect modifiers
    public int GetEffectiveAttack()
    {
        var modifier = 1.0;
        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.Type == StatusEffectType.AttackUp) modifier += effect.Modifier;
            if (effect.Type == StatusEffectType.AttackDown) modifier -= effect.Modifier;
        }
        return Math.Max(1, (int)(Attack * modifier));
    }

    public int GetEffectiveDefense()
    {
        var modifier = 1.0;
        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.Type == StatusEffectType.DefenseUp) modifier += effect.Modifier;
            if (effect.Type == StatusEffectType.DefenseDown) modifier -= effect.Modifier;
        }
        return Math.Max(0, (int)(Defense * modifier));
    }

    public int GetEffectiveTechAttack()
    {
        var modifier = 1.0;
        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.Type == StatusEffectType.TechAttackUp) modifier += effect.Modifier;
            if (effect.Type == StatusEffectType.TechAttackDown) modifier -= effect.Modifier;
        }
        return Math.Max(1, (int)(TechAttack * modifier));
    }

    public int GetEffectiveTechDefense()
    {
        var modifier = 1.0;
        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.Type == StatusEffectType.TechDefenseUp) modifier += effect.Modifier;
            if (effect.Type == StatusEffectType.TechDefenseDown) modifier -= effect.Modifier;
        }
        return Math.Max(0, (int)(TechDefense * modifier));
    }

    public double GetAccuracyModifier()
    {
        var modifier = 1.0;
        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.Type == StatusEffectType.AccuracyUp) modifier += effect.Modifier;
            if (effect.Type == StatusEffectType.AccuracyDown) modifier -= effect.Modifier;
        }
        return Math.Max(0.1, Math.Min(1.5, modifier));
    }

    public double GetEvasionModifier()
    {
        var modifier = 0.0;
        foreach (var effect in ActiveStatusEffects)
        {
            if (effect.Type == StatusEffectType.EvasionUp) modifier += effect.Modifier;
            if (effect.Type == StatusEffectType.EvasionDown) modifier -= effect.Modifier;
        }
        return Math.Max(-0.3, Math.Min(0.3, modifier));
    }

    public bool HasHealBlock()
    {
        return ActiveStatusEffects.Any(e => e.Type == StatusEffectType.HealBlock);
    }

    public bool IsStunned()
    {
        return ActiveStatusEffects.Any(e => e.Type == StatusEffectType.Stunned);
    }

    public bool IsProtected()
    {
        return ActiveStatusEffects.Any(e => e.Type == StatusEffectType.Protected);
    }

    public StatusEffect? GetChargingEffect()
    {
        return ActiveStatusEffects.FirstOrDefault(e => e.Type == StatusEffectType.Charging);
    }
}
