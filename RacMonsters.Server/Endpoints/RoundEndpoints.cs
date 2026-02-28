using RacMonsters.Server.Services.Rounds;

namespace RacMonsters.Server.Endpoints;

public static class RoundEndpoints
{
    public static void MapRoundEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("round/create", async (IRoundService svc) =>
        {
            var res = svc.CreateSession(req ?? new CreateSessionRequest(null));
            return Results.Ok(res);
        });
    }
}
