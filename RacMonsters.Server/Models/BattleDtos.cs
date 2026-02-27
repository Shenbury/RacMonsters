namespace RacMonsters.Server.Models;

// Session and turn-oriented DTOs
public record ActionRequest(int ActorId, string Action, int? TargetId, int? AbilityIndex);

public record CreateSessionRequest(int[]? ParticipantIds);
public record CreateSessionResult(Guid SessionId, List<Character> Characters);

public record TurnEvent(
    int TurnNumber,
    string Action,
    int ActorId,
    string ActorName,
    int? TargetId,
    string? TargetName,
    int? AbilityIndex,
    string? AbilityName,
    int Damage,
    int Heal,
    int ActorHpAfter,
    int? TargetHpAfter,
    DateTime Timestamp
);

public record BattleTurnResult(Guid SessionId, int TurnNumber, List<Character> Characters, List<TurnEvent> Events);

