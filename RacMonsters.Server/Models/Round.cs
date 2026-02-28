namespace RacMonsters.Server.Models
{
    public class Round
    {
        public int Id { get; set; }
        public (Character CharacterA, Ability AbilityA) PlayerA { get; set; }
        public (Character CharacterB, Ability AbilityB) PlayerB { get; set; }
    }
}
