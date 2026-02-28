using RacMonsters.Server.Services.Abilities;
using RacMonsters.Server.Services.Characters;

namespace RacMonsters.Server.Endpoints;

public static class AbilityEndpoints
{
    public static void MapAbilityEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("characters/{id}", (int id, ICharacterService svc) =>
        {
            var ch = svc.GetAll().FirstOrDefault(c => c.Id == id);
            return ch is null ? Results.NotFound() : Results.Ok(ch);
        });
    }
}
