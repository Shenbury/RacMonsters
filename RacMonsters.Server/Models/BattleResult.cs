namespace RacMonsters.Server.Models
{
    public class BattleResult
    {
        public int BattleId { get; set; }
        public Character PlayerCharacter { get; set; } = null!;
        public Character OpponentCharacter { get; set; } = null!;
        public Round? LastRound { get; set; }
        public bool IsGameOver { get; set; }
        public string? Winner { get; set; }
        public string? Message { get; set; }

        // Fields to identify which player triggered the event
        public string? TriggeringPlayerConnectionId { get; set; }
        public string? TriggeringPlayerName { get; set; }
        public Character? TriggeringPlayerCharacter { get; set; }
    }
}
