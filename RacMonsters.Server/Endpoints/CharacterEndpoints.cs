using RacMonsters.Server.Services.Characters;

namespace RacMonsters.Server.Endpoints;

public static class CharacterEndpoints
{
    public static void MapCharacterEndpoints(this RouteGroupBuilder api)
    {
        // Return the list of characters (resolved from ICharacterService)
        api.MapGet("characters", (ICharacterService svc) => Results.Ok(svc.GetAll()));

        api.MapGet("characters/{id}", (int id, ICharacterService svc) =>
        {
            var ch = svc.GetAll().FirstOrDefault(c => c.Id == id);
            return ch is null ? Results.NotFound() : Results.Ok(ch);
        });
    }
}
