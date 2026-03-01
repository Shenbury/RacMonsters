using Microsoft.EntityFrameworkCore;
using RacMonsters.Server.Data;
using RacMonsters.Server.Models;
using System.Text.Json;

namespace RacMonsters.Server.Repositories.Sessions
{
    public class SqlSessionRepository : ISessionRepository
    {
        private readonly RacMonstersDbContext _db;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };

        public SqlSessionRepository(RacMonstersDbContext db)
        {
            _db = db;
        }

        public async Task<Session> CreateSession(Session createSession)
        {
            var entity = new SessionEntity();
            _db.Sessions.Add(entity);
            await _db.SaveChangesAsync();

            createSession.Id = entity.Id;

            // if there are battles on the provided session, persist them
            if (createSession.Battles != null && createSession.Battles.Length > 0)
            {
                foreach (var b in createSession.Battles)
                {
                    var be = new BattleEntity
                    {
                        SessionId = entity.Id,
                        CharacterAJson = JsonSerializer.Serialize(b.CharacterA, _jsonOptions),
                        CharacterBJson = JsonSerializer.Serialize(b.CharacterB, _jsonOptions),
                        WinningCharacter = b.WinningCharacter
                    };
                    _db.Battles.Add(be);
                    await _db.SaveChangesAsync();

                    if (b.Rounds != null)
                    {
                        foreach (var r in b.Rounds)
                        {
                            var re = new RoundEntity
                            {
                                BattleId = be.Id,
                                PlayerAJson = JsonSerializer.Serialize(r.PlayerA, _jsonOptions),
                                PlayerBJson = JsonSerializer.Serialize(r.PlayerB, _jsonOptions)
                            };
                            _db.Rounds.Add(re);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }

            return createSession;
        }

        public async Task<Session> GetSession(int sessionId)
        {
            var e = await _db.Sessions
                .Include(s => s.Battles)
                    .ThenInclude(b => b.Rounds)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (e == null) return null!;

            var s = new Session { Id = e.Id };
            var battles = new List<Battle>();

            foreach (var be in e.Battles.OrderBy(b => b.Id))
            {
                var battle = new Battle
                {
                    Id = be.Id,
                    CharacterA = JsonSerializer.Deserialize<Character>(be.CharacterAJson, _jsonOptions)!,
                    CharacterB = JsonSerializer.Deserialize<Character>(be.CharacterBJson, _jsonOptions)!,
                    WinningCharacter = be.WinningCharacter,
                    Rounds = be.Rounds.OrderBy(r => r.Id).Select(re => JsonSerializer.Deserialize<Round>("{\"PlayerA\":" + re.PlayerAJson + ",\"PlayerB\":" + re.PlayerBJson + "}", _jsonOptions)!).ToArray()
                };

                // Because Round deserialization above expects a Round object with PlayerA and PlayerB as RoundAction,
                // ensure PlayerA/PlayerB are deserialized correctly by manually constructing if needed.
                // The simple approach above concatenates JSON snippets into a Round-shaped JSON.

                battles.Add(battle);
            }

            s.Battles = battles.ToArray();
            return s;
        }

        public async Task<Session> UpdateSession(Session session)
        {
            // Upsert session
            var existing = await _db.Sessions
                .Include(s => s.Battles)
                    .ThenInclude(b => b.Rounds)
                .FirstOrDefaultAsync(s => s.Id == session.Id);

            if (existing == null)
            {
                existing = new SessionEntity();
                _db.Sessions.Add(existing);
                await _db.SaveChangesAsync();
                session.Id = existing.Id;
            }

            // remove existing battles & rounds and re-add from session model for simplicity
            var existingBattles = existing.Battles.ToList();
            foreach (var eb in existingBattles)
            {
                // remove rounds
                var rounds = _db.Rounds.Where(r => r.BattleId == eb.Id);
                _db.Rounds.RemoveRange(rounds);
                _db.Battles.Remove(eb);
            }
            await _db.SaveChangesAsync();

            if (session.Battles != null)
            {
                foreach (var b in session.Battles)
                {
                    var be = new BattleEntity
                    {
                        SessionId = existing.Id,
                        CharacterAJson = JsonSerializer.Serialize(b.CharacterA, _jsonOptions),
                        CharacterBJson = JsonSerializer.Serialize(b.CharacterB, _jsonOptions),
                        WinningCharacter = b.WinningCharacter
                    };
                    _db.Battles.Add(be);
                    await _db.SaveChangesAsync();

                    if (b.Rounds != null)
                    {
                        foreach (var r in b.Rounds)
                        {
                            var re = new RoundEntity
                            {
                                BattleId = be.Id,
                                PlayerAJson = JsonSerializer.Serialize(r.PlayerA, _jsonOptions),
                                PlayerBJson = JsonSerializer.Serialize(r.PlayerB, _jsonOptions)
                            };
                            _db.Rounds.Add(re);
                        }
                        await _db.SaveChangesAsync();
                    }
                }
            }

            return session;
        }
    }
}
