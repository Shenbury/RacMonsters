namespace RacMonsters.Server.Models;

public class Battle
{
    public int Id { get; set; }
    public Character CharacterA { get; set; }
    public Character CharacterB { get; set; }
    public Round[] Rounds { get; set; } = Array.Empty<Round>();
    public string? WinningCharacter { get; set; } = null;

}
