using RacMonsters.Server.Models;
using RacMonsters.Server.Services.Leaderboard;

namespace RacMonsters.Server.Endpoints;

public static class LeaderboardEndpoints
{
    public static void MapLeaderboardEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("leaderboard/top", async (int? limit, ILeaderboardService svc) =>
        {
            var l = await svc.GetTop(limit ?? 20);
            return Results.Ok(l);
        });

        api.MapPost("leaderboard", async (LeaderboardEntry entry, ILeaderboardService svc) =>
        {
            var res = await svc.AddEntry(entry);
            return Results.Ok(res);
        });

        api.MapPost("leaderboard/upsert", async (string name, int delta, ILeaderboardService svc) =>
        {
            var res = await svc.Upsert(name, delta);
            return Results.Ok(res);
        });
    }
}
