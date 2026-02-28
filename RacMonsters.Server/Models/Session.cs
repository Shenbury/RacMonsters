namespace RacMonsters.Server.Models
{
    public class Session
    {
        public int Id { get; set; }
        public Battle[] Battles { get; set; }
    }
}
