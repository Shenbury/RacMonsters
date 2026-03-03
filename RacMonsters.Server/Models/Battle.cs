namespace RacMonsters.Server.Models;

public class Battle
{
    public int Id { get; set; }
    public Character CharacterA { get; set; }
    public Character CharacterB { get; set; }
    public Round[] Rounds { get; set; } = Array.Empty<Round>();
    public string? WinningCharacter { get; set; } = null;

    // Multiplayer properties (Phase 4)
    public bool IsMultiplayer { get; set; }
    public string? Player1ConnectionId { get; set; }
    public string? Player2ConnectionId { get; set; }
    public string? CurrentTurnConnectionId { get; set; }
    public DateTime? TurnStartTime { get; set; }
    public int TurnTimeoutSeconds { get; set; } = 30;

    // For simultaneous turn-based combat with speed resolution
    public int? Player1SelectedAbilityId { get; set; }
    public int? Player2SelectedAbilityId { get; set; }
    public bool Player1Ready { get; set; }
    public bool Player2Ready { get; set; }
}
