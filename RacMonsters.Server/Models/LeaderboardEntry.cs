namespace RacMonsters.Server.Models
{
    public class LeaderboardEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
