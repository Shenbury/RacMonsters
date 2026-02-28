using RacMonsters.Server.Services.Battles;
using RacMonsters.Server.Services.Sessions;

namespace RacMonsters.Server.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("session", async (ISessionService svc) =>
        {
            var res = svc.CreateSession(req ?? new CreateSessionRequest(null));
            return Results.Ok(res);
        });
    }
}
