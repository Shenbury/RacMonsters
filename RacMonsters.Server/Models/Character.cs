namespace RacMonsters.Server.Models;

public class Character
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string ImageUrl { get; init; } = string.Empty;

    public int CurrentHealth { get; set; }
    public int MaxHealth { get; init; }

    public Ability[] Abilities { get; init; } = [];

    public int Attack { get; init; }
    public int Defense { get; init; }
    public int TechAttack { get; init; }
    public int TechDefense { get; init; }
}
