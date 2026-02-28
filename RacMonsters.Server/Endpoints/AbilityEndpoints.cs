using RacMonsters.Server.Services.Abilities;
using RacMonsters.Server.Models;

namespace RacMonsters.Server.Endpoints;

public static class AbilityEndpoints
{
    public static void MapAbilityEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("ability/{id}", async (int id, IAbilityService svc) =>
        {
            var arr = await svc.GetAbilities(new[] { id });
            var a = arr.FirstOrDefault();
            return a is null ? Results.NotFound() : Results.Ok(a);
        });
    }
}
