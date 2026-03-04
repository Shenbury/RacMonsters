namespace RacMonsters.Server.Models;

public class Battle
{
    public int Id { get; set; }

    // Standard 1v1 mode properties (backward compatible)
    public Character CharacterA { get; set; }
    public Character CharacterB { get; set; }

    // Team Battle mode properties
    public BattleMode Mode { get; set; } = BattleMode.Standard;
    public List<Character>? Team1Characters { get; set; }
    public List<Character>? Team2Characters { get; set; }
    public int ActiveTeam1CharacterIndex { get; set; } = 0;
    public int ActiveTeam2CharacterIndex { get; set; } = 0;

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

    // Helper methods for team battles
    public Character GetActiveTeam1Character()
    {
        return Mode == BattleMode.TeamBattle && Team1Characters != null 
            ? Team1Characters[ActiveTeam1CharacterIndex] 
            : CharacterA;
    }

    public Character GetActiveTeam2Character()
    {
        return Mode == BattleMode.TeamBattle && Team2Characters != null 
            ? Team2Characters[ActiveTeam2CharacterIndex] 
            : CharacterB;
    }

    public bool IsTeam1Defeated()
    {
        if (Mode == BattleMode.Standard)
            return CharacterA.CurrentHealth <= 0;

        return Team1Characters?.All(c => c.CurrentHealth <= 0) ?? false;
    }

    public bool IsTeam2Defeated()
    {
        if (Mode == BattleMode.Standard)
            return CharacterB.CurrentHealth <= 0;

        return Team2Characters?.All(c => c.CurrentHealth <= 0) ?? false;
    }
}

