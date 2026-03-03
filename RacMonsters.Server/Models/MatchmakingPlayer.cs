namespace RacMonsters.Server.Models
{
    public class MatchmakingPlayer
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public int CharacterId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
