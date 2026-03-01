using System.Linq;
using RacMonsters.Server.Services.Rounds;
using RacMonsters.Server.Models;
using RacMonsters.Server.Services.Battles;

namespace RacMonsters.Server.Endpoints;

public static class RoundEndpoints
{
    public static void MapRoundEndpoints(this RouteGroupBuilder api)
    {
        // Execute a round and attach it to the current session's most recent battle (or create a new battle entry)
        api.MapPost("round/execute", async (HttpContext context, Round round, IRoundService svc, IBattleService battleSvc, RacMonsters.Server.Services.Sessions.ISessionService sessionSvc) =>
        {
            var res = await svc.ExecuteRound(round);

            if (context.Items.TryGetValue("Session", out var sessObj) && sessObj is RacMonsters.Server.Models.Session sess)
            {
                // find the most recent battle in the session
                var last = sess.Battles.LastOrDefault();

                if (last == null)
                {
                    // create a new battle record for this session using the round participants
                    var newBattle = new RacMonsters.Server.Models.Battle
                    {
                        CharacterA = round.PlayerA.Character,
                        CharacterB = round.PlayerB.Character,
                        Rounds = new[] { res }
                    };
                    var created = await battleSvc.CreateBattle(newBattle);
                    sess.Battles = sess.Battles.Append(created).ToArray();
                }
                else
                {
                    // append round to the existing last battle
                    last.Rounds = last.Rounds.Append(res).ToArray();
                }

                // persist session changes
                await sessionSvc.UpdateSession(sess);
            }

            return Results.Ok(res);
        });
    }
}
