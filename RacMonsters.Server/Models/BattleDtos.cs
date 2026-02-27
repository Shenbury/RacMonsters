namespace RacMonsters.Server.Models;

public record AttackRequest(int AttackerId, int DefenderId, int? AbilityIndex);
public record BattleResult(int DefenderId, int DefenderHp, string Message);
public record HealRequest(int TargetId);
