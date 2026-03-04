namespace RacMonsters.Server.Models
{
    public class StatusEffectApplication
    {
        public StatusEffectType Type { get; set; }
        public int Duration { get; set; }
        public int Power { get; set; }
        public double Modifier { get; set; } = 1.0;
        public double ApplyChance { get; set; } = 1.0;  // Probability of applying (0.0 to 1.0)
        public bool ApplyToSelf { get; set; } = false;  // True for buffs, false for debuffs/DoTs
        public bool RequiresCharging { get; set; } = false;  // True if this is a charging move
    }
}
