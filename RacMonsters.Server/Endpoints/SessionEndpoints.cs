using RacMonsters.Server.Services.Sessions;

namespace RacMonsters.Server.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("session/create", async (ISessionService svc) =>
        {
            var res = svc.CreateSession(req ?? new CreateSessionRequest(null));
            return Results.Ok(res);
        });

        api.MapGet("session/{id}", (int id, ISessionService svc) =>
        {
            var ch = svc.GetAll().FirstOrDefault(c => c.Id == id);
            return ch is null ? Results.NotFound() : Results.Ok(ch);
        });
    }
}
