using RacMonsters.Server.Services.Battles;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Endpoints;

public static class BattleEndpoints
{
    public static void MapBattleEndpoints(this RouteGroupBuilder api)
    {
        // Create a battle and automatically attach it to the current session (if present on HttpContext.Items)
        api.MapPost("battle/create", async (HttpContext context, Battle battle, IBattleService svc, RacMonsters.Server.Services.Sessions.ISessionService sessionSvc) =>
        {
            var res = await svc.CreateBattle(battle);

            if (context.Items.TryGetValue("Session", out var sessObj) && sessObj is RacMonsters.Server.Models.Session sess)
            {
                // append to session battles
                sess.Battles = sess.Battles.Append(res).ToArray();
                // persist session changes
                await sessionSvc.UpdateSession(sess);
            }

            return Results.Ok(res);
        });

        api.MapGet("battle/{id}", async (int id, IBattleService svc) =>
        {
            var b = await svc.GetBattle(id);
            return b is null ? Results.NotFound() : Results.Ok(b);
        });
    }
}
