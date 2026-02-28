namespace RacMonsters.Server.Models
{
    public class Round
    {
        public int Id { get; set; }
        // actions chosen by both participants in this round
        public RoundAction PlayerA { get; set; } = new RoundAction();
        public RoundAction PlayerB { get; set; } = new RoundAction();
    }
}
