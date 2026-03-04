namespace RacMonsters.Server.Models;

public class RoundAction
{
    public ActionType Type { get; set; } = ActionType.Ability;
    public Character Character { get; set; } = null!;
    public Ability Ability { get; set; } = null!;

    // For switch actions
    public int? SwitchToCharacterIndex { get; set; }

    // Whether the chosen ability hit its target (or succeeded for heals)
    public bool? Hit { get; set; }

    // Optional human-friendly message describing the result of the action (hit/miss, damage, heal)
    public string? ResultMessage { get; set; }
    // Structured numeric fields for frontend consumption
    public int? Damage { get; set; }
    public int? HealAmount { get; set; }

    // Status effects applied this round
    public List<string> StatusEffectMessages { get; set; } = new List<string>();
}
