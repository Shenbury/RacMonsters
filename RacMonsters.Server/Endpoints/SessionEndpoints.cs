using RacMonsters.Server.Models;
using RacMonsters.Server.Services.Sessions;

namespace RacMonsters.Server.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("session/create", async (Session session, ISessionService svc) =>
        {
            var res = await svc.CreateSession(session);
            return Results.Ok(res);
        });

        api.MapGet("session/{id}", async (int id, ISessionService svc) =>
        {
            var s = await svc.GetSession(id);
            return s is null ? Results.NotFound() : Results.Ok(s);
        });
    }
}
