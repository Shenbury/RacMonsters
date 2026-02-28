using RacMonsters.Server.Services.Battles;

namespace RacMonsters.Server.Endpoints;

public static class BattleEndpoints
{
    public static void MapBattleEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("battle/create", async (IBattleService svc) =>
        {
            var res = svc.CreateSession(req ?? new CreateSessionRequest(null));
            return Results.Ok(res);
        });
    }
}
