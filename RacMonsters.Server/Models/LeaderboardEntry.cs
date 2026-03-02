namespace RacMonsters.Server.Models
{
    public class LeaderboardEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // name of the character/champion the player used
        public string Character { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
