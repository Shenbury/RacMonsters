namespace RacMonsters.Server.Models
{
    public class Ability
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public bool IsTech { get; init; }
        public bool IsHeal { get; init; } = false;
        public int Power { get; init; }
        public int Speed { get; init; }
        public double Accuracy { get; init; }

        // Status effects this ability can apply
        public List<StatusEffectApplication> StatusEffects { get; init; } = new List<StatusEffectApplication>();

        // Helper properties
        public bool IsChargingMove => StatusEffects.Any(se => se.RequiresCharging);
        public bool AppliesStatusEffects => StatusEffects.Any();
    }
}
