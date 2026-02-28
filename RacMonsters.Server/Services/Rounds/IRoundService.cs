using RacMonsters.Server.Models;

namespace RacMonsters.Server.Services.Rounds
{
    public interface IRoundService
    {
        Task<Round> ExecuteRound(Round r);
    }
}
