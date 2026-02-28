using RacMonsters.Server.Services.Battles;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Endpoints;

public static class BattleEndpoints
{
    public static void MapBattleEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("battle/create", async (Battle battle, IBattleService svc) =>
        {
            var res = await svc.CreateBattle(battle);
            return Results.Ok(res);
        });

        api.MapGet("battle/{id}", async (int id, IBattleService svc) =>
        {
            var b = await svc.GetBattle(id);
            return b is null ? Results.NotFound() : Results.Ok(b);
        });
    }
}
