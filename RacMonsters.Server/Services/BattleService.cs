using RacMonsters.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RacMonsters.Server.Services;

public interface IBattleService
{
    CreateSessionResult CreateSession(CreateSessionRequest req);
    BattleTurnResult ProcessAction(System.Guid sessionId, ActionRequest req);
    BattleTurnResult? GetSessionState(System.Guid sessionId);
}

public class BattleService : IBattleService
{
    private readonly ICharacterService _chars;
    private readonly Random _rng = new();
    private readonly Dictionary<System.Guid, SessionState> _sessions = new();

    private class SessionState
    {
        public System.Guid Id { get; init; }
        public List<Character> Characters { get; init; } = new();
        public List<TurnEvent> History { get; init; } = new();
        public int TurnNumber { get; set; }
    }

    public BattleService(ICharacterService chars)
    {
        _chars = chars;
    }

    private Character Clone(Character src)
    {
        return new Character
        {
            Id = src.Id,
            Name = src.Name,
            Health = src.Health,
            MaxHealth = src.MaxHealth,
            Abilities = src.Abilities ?? System.Array.Empty<Ability>(),
            ImageUrl = src.ImageUrl,
            Attack = src.Attack,
            Defense = src.Defense,
            TechAttack = src.TechAttack,
            TechDefense = src.TechDefense
        };
    }

    public CreateSessionResult CreateSession(CreateSessionRequest req)
    {
        var all = _chars.GetAll();
        List<Character> participants;
        if (req.ParticipantIds != null && req.ParticipantIds.Length > 0)
        {
            participants = all.Where(c => req.ParticipantIds.Contains(c.Id)).Select(Clone).ToList();
        }
        else
        {
            // default: first two characters
            participants = all.Take(2).Select(Clone).ToList();
        }

        var id = System.Guid.NewGuid();
        var st = new SessionState { Id = id, Characters = participants, TurnNumber = 0 };
        _sessions[id] = st;
        return new CreateSessionResult(id, participants);
    }

    public BattleTurnResult? GetSessionState(System.Guid sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var st)) return null;
        return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters, st.History.ToList());
    }

    public BattleTurnResult ProcessAction(System.Guid sessionId, ActionRequest req)
    {
        if (!_sessions.TryGetValue(sessionId, out var st))
        {
            // invalid session
            return new BattleTurnResult(sessionId, 0, new List<Character>(), new List<TurnEvent> { new TurnEvent(0, "error", req.ActorId, "", null, null, null, null, 0, 0, 0, null, System.DateTime.UtcNow) });
        }

        st.TurnNumber++;
        var turn = st.TurnNumber;
        var events = new List<TurnEvent>();

        var actor = st.Characters.FirstOrDefault(c => c.Id == req.ActorId);
        if (actor is null)
        {
            var ev = new TurnEvent(turn, "error", req.ActorId, "", null, null, null, null, 0, 0, 0, null, System.DateTime.UtcNow);
            st.History.Add(ev);
            return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
        }

        if (req.Action == "attack")
        {
            if (req.TargetId is not int targetId)
            {
                var ev = new TurnEvent(turn, "error", actor.Id, actor.Name, null, null, null, null, 0, 0, actor.Health, null, System.DateTime.UtcNow);
                st.History.Add(ev);
                return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
            }

            var defender = st.Characters.FirstOrDefault(c => c.Id == targetId);
            if (defender is null)
            {
                var ev = new TurnEvent(turn, "error", actor.Id, actor.Name, targetId, null, null, null, 0, 0, actor.Health, null, System.DateTime.UtcNow);
                st.History.Add(ev);
                return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
            }

            Ability? ability = null;
            string? abilityName = null;
            int? abilityIndex = null;
            if (req.AbilityIndex is int idx)
            {
                if (idx < 0 || idx >= actor.Abilities.Length)
                {
                    var ev = new TurnEvent(turn, "error", actor.Id, actor.Name, defender.Id, defender.Name, null, null, 0, 0, actor.Health, defender.Health, System.DateTime.UtcNow);
                    st.History.Add(ev);
                    return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
                }
                ability = actor.Abilities[idx];
                abilityName = ability.Name;
                abilityIndex = idx;
            }

            var attackStat = ability?.IsTech == true ? actor.TechAttack : actor.Attack;
            var defenseStat = ability?.IsTech == true ? defender.TechDefense : defender.Defense;
            var basePower = ability?.Power ?? 8;
            var variance = _rng.Next(-3, 4);
            var raw = Math.Max(1, basePower + attackStat - (int)(defenseStat * 0.6) + variance);
            defender.Health = Math.Max(0, defender.Health - raw);

            var ev2 = new TurnEvent(turn, "attack", actor.Id, actor.Name, defender.Id, defender.Name, abilityIndex, abilityName, raw, 0, actor.Health, defender.Health, System.DateTime.UtcNow);
            st.History.Add(ev2);
            events.Add(ev2);
        }
        else if (req.Action == "heal")
        {
            if (req.TargetId is not int targetId)
            {
                var ev = new TurnEvent(turn, "error", actor.Id, actor.Name, null, null, null, null, 0, 0, actor.Health, null, System.DateTime.UtcNow);
                st.History.Add(ev);
                return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
            }

            var target = st.Characters.FirstOrDefault(c => c.Id == targetId);
            if (target is null)
            {
                var ev = new TurnEvent(turn, "error", actor.Id, actor.Name, targetId, null, null, null, 0, 0, actor.Health, null, System.DateTime.UtcNow);
                st.History.Add(ev);
                return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
            }

            var heal = _rng.Next(8, 16);
            target.Health = Math.Min(target.MaxHealth, target.Health + heal);
            var ev2 = new TurnEvent(turn, "heal", actor.Id, actor.Name, target.Id, target.Name, null, null, 0, heal, actor.Health, target.Health, System.DateTime.UtcNow);
            st.History.Add(ev2);
            events.Add(ev2);
        }
        else
        {
            var ev = new TurnEvent(turn, "error", actor.Id, actor.Name, null, null, null, null, 0, 0, actor.Health, null, System.DateTime.UtcNow);
            st.History.Add(ev);
            return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), st.History.ToList());
        }

        // AI response: choose first alive opponent that's not the actor
        var possibleTargets = st.Characters.Where(c => c.Id != req.ActorId && c.Health > 0).ToList();
        var actorAlive = st.Characters.Any(c => c.Id == req.ActorId && c.Health > 0);
        if (possibleTargets.Count > 0 && actorAlive)
        {
            var ai = possibleTargets.First();
            var aiTarget = st.Characters.FirstOrDefault(c => c.Id == req.ActorId);
            if (aiTarget != null && ai.Health > 0)
            {
                var abilityChoice = (ai.Abilities?.Length ?? 0) > 0 && _rng.NextDouble() > 0.6 ? _rng.Next(0, ai.Abilities.Length) : (int?)null;
                if (ai.Abilities != null && abilityChoice is int aiIdx)
                {
                    var ability = ai.Abilities[aiIdx];
                    var attackStat = ability.IsTech ? ai.TechAttack : ai.Attack;
                    var defenseStat = ability.IsTech ? aiTarget.TechDefense : aiTarget.Defense;
                    var basePower = ability.Power;
                    var variance = _rng.Next(-3, 4);
                    var raw = Math.Max(1, basePower + attackStat - (int)(defenseStat * 0.6) + variance);
                    aiTarget.Health = Math.Max(0, aiTarget.Health - raw);
                    var ev3 = new TurnEvent(turn, "attack", ai.Id, ai.Name, aiTarget.Id, aiTarget.Name, aiIdx, ability.Name, raw, 0, ai.Health, aiTarget.Health, System.DateTime.UtcNow);
                    st.History.Add(ev3);
                    events.Add(ev3);
                }
                else
                {
                    var basePower = 8;
                    var variance = _rng.Next(-3, 4);
                    var raw = Math.Max(1, basePower + ai.Attack - (int)(aiTarget.Defense * 0.6) + variance);
                    aiTarget.Health = Math.Max(0, aiTarget.Health - raw);
                    var ev3 = new TurnEvent(turn, "attack", ai.Id, ai.Name, aiTarget.Id, aiTarget.Name, null, null, raw, 0, ai.Health, aiTarget.Health, System.DateTime.UtcNow);
                    st.History.Add(ev3);
                    events.Add(ev3);
                }
            }
        }

        return new BattleTurnResult(st.Id, st.TurnNumber, st.Characters.ToList(), events);
    }
}
