using RacMonsters.Server.Services.Rounds;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Endpoints;

public static class RoundEndpoints
{
    public static void MapRoundEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("round/execute", async (Round round, IRoundService svc) =>
        {
            var res = await svc.ExecuteRound(round);
            return Results.Ok(res);
        });
    }
}
