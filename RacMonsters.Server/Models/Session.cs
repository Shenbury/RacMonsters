namespace RacMonsters.Server.Models
{
    public class Session
    {
        public int Id { get; set; }
        // battles contained in this session
        public Battle[] Battles { get; set; } = Array.Empty<Battle>();
    }
}
