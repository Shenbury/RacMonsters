using Microsoft.AspNetCore.Builder;
using RacMonsters.Server.Services;
using RacMonsters.Server.Models;
using Microsoft.AspNetCore.Routing;
using System;

namespace RacMonsters.Server.Endpoints;

public static class BattleEndpoints
{
    public static void MapBattleEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("battle/session", (CreateSessionRequest req, IBattleService svc) =>
        {
            var res = svc.CreateSession(req ?? new CreateSessionRequest(null));
            return Results.Ok(res);
        });

        api.MapGet("battle/session/{id:guid}", (Guid id, IBattleService svc) =>
        {
            var state = svc.GetSessionState(id);
            return state is null ? Results.NotFound() : Results.Ok(state);
        });

        api.MapPost("battle/session/{id:guid}/action", (Guid id, ActionRequest req, IBattleService svc) =>
        {
            var res = svc.ProcessAction(id, req);
            return Results.Ok(res);
        });
    }
}
