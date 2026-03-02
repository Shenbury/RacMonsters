using Microsoft.EntityFrameworkCore;
using RacMonsters.Server.Data;
using RacMonsters.Server.Models;
using RacMonsters.Server.Repositories.Characters;
using RacMonsters.Server.Repositories.Abilities;

namespace RacMonsters.Server.Repositories.Sessions
{
    public class SqlSessionRepository : ISessionRepository
    {
        private readonly RacMonstersDbContext _db;
        private readonly ICharacterRepository _characterRepo;
        private readonly IAbilityRepository _abilityRepo;

        public SqlSessionRepository(RacMonstersDbContext db, ICharacterRepository characterRepo, IAbilityRepository abilityRepo)
        {
            _db = db;
            _characterRepo = characterRepo;
            _abilityRepo = abilityRepo;
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
                        CharacterAId = b.CharacterA.Id,
                        CharacterBId = b.CharacterB.Id,
                        WinningCharacterId = null
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
                                PlayerACharacterId = r.PlayerA.Character.Id,
                                PlayerAAbilityId = r.PlayerA.Ability.Id,
                                PlayerAHit = r.PlayerA.Hit,
                                PlayerAResultMessage = r.PlayerA.ResultMessage,
                                PlayerADamage = r.PlayerA.Damage,
                                PlayerAHealAmount = r.PlayerA.HealAmount,

                                PlayerBCharacterId = r.PlayerB.Character.Id,
                                PlayerBAbilityId = r.PlayerB.Ability.Id,
                                PlayerBHit = r.PlayerB.Hit,
                                PlayerBResultMessage = r.PlayerB.ResultMessage,
                                PlayerBDamage = r.PlayerB.Damage,
                                PlayerBHealAmount = r.PlayerB.HealAmount
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
                .Include(s => s.Battles)
                    .ThenInclude(b => b.CharacterA)
                .Include(s => s.Battles)
                    .ThenInclude(b => b.CharacterB)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (e == null) return null!;

            var s = new Session { Id = e.Id };
            var battles = new List<Battle>();

            foreach (var be in e.Battles.OrderBy(b => b.Id))
            {
                // load characters via character repository to include abilities
                var charA = await _characterRepo.GetCharacter(be.CharacterAId);
                var charB = await _characterRepo.GetCharacter(be.CharacterBId);

                var rounds = new List<Round>();
                foreach (var re in be.Rounds.OrderBy(r => r.Id))
                {
                    var playerAAbility = await _abilityRepo.GetAbilities(new[] { re.PlayerAAbilityId });
                    var playerBAbility = await _abilityRepo.GetAbilities(new[] { re.PlayerBAbilityId });

                    var round = new Round
                    {
                        Id = re.Id,
                        PlayerA = new RoundAction
                        {
                            Character = charA,
                            Ability = playerAAbility.FirstOrDefault()!,
                            Hit = re.PlayerAHit,
                            ResultMessage = re.PlayerAResultMessage,
                            Damage = re.PlayerADamage,
                            HealAmount = re.PlayerAHealAmount
                        },
                        PlayerB = new RoundAction
                        {
                            Character = charB,
                            Ability = playerBAbility.FirstOrDefault()!,
                            Hit = re.PlayerBHit,
                            ResultMessage = re.PlayerBResultMessage,
                            Damage = re.PlayerBDamage,
                            HealAmount = re.PlayerBHealAmount
                        }
                    };

                    rounds.Add(round);
                }

                var battle = new Battle
                {
                    Id = be.Id,
                    CharacterA = charA,
                    CharacterB = charB,
                    WinningCharacter = be.WinningCharacterEntity != null ? be.WinningCharacterEntity.Name : null,
                    Rounds = rounds.ToArray()
                };

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
                        CharacterAId = b.CharacterA.Id,
                        CharacterBId = b.CharacterB.Id,
                        WinningCharacterId = null
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
                                PlayerACharacterId = r.PlayerA.Character.Id,
                                PlayerAAbilityId = r.PlayerA.Ability.Id,
                                PlayerAHit = r.PlayerA.Hit,
                                PlayerAResultMessage = r.PlayerA.ResultMessage,
                                PlayerADamage = r.PlayerA.Damage,
                                PlayerAHealAmount = r.PlayerA.HealAmount,

                                PlayerBCharacterId = r.PlayerB.Character.Id,
                                PlayerBAbilityId = r.PlayerB.Ability.Id,
                                PlayerBHit = r.PlayerB.Hit,
                                PlayerBResultMessage = r.PlayerB.ResultMessage,
                                PlayerBDamage = r.PlayerB.Damage,
                                PlayerBHealAmount = r.PlayerB.HealAmount
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
