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
    }
}
