namespace RacMonsters.Server.Models;

public record Ability(string Name, int Power, bool IsTech);

public class Character
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int Health { get; set; }
    public int MaxHealth { get; init; }
    public Ability[] Abilities { get; init; } = Array.Empty<Ability>();
    public string ImageUrl { get; init; } = string.Empty;
    public int Attack { get; init; }
    public int Defense { get; init; }
    public int TechAttack { get; init; }
    public int TechDefense { get; init; }
}
