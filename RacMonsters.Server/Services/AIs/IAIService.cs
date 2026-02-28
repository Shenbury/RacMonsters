namespace RacMonsters.Server.Services.AIs
{
    using RacMonsters.Server.Models;

    public interface IAIService
    {
        /// <summary>
        /// Choose an ability for the provided character (AI decision).
        /// </summary>
        Task<Ability> ChooseAbilityAsync(Character character);
    }
}
