using RacMonsters.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var api = app.MapGroup("/api");

var characters = new List<RacMonsters.Server.Models.Character>
{
    new RacMonsters.Server.Models.Character
    {
        Id = 1,
        Name = "Embermon",
        Health = 100,
        MaxHealth = 100,
        Abilities = new[] {
            new RacMonsters.Server.Models.Ability("Flame Strike", 18, false),
            new RacMonsters.Server.Models.Ability("Heat Wave", 26, true),
            new RacMonsters.Server.Models.Ability("Singe", 10, false)
        },
        ImageUrl = "/images/embermon.svg",
        Attack = 24,
        Defense = 12,
        TechAttack = 18,
        TechDefense = 10
    },
    new RacMonsters.Server.Models.Character
    {
        Id = 2,
        Name = "Sam",
        Health = 120,
        MaxHealth = 120,
        Abilities = new[] {
            new RacMonsters.Server.Models.Ability("Tidal Punch", 16, false),
            new RacMonsters.Server.Models.Ability("Aqua Burst", 22, true),
            new RacMonsters.Server.Models.Ability("Shell Guard", 0, false)
        },
        ImageUrl = "/public/Sam.svg",
        Attack = 18,
        Defense = 20,
        TechAttack = 14,
        TechDefense = 18
    }
};

api.MapGet("characters", () => Results.Ok(characters));

api.MapGet("characters/{id}", (int id) =>
{
    var ch = characters.FirstOrDefault(c => c.Id == id);
    return ch is null ? Results.NotFound() : Results.Ok(ch);
});

// DTOs moved to RacMonsters.Server.Models

api.MapPost("battle/attack", (AttackRequest req) =>
{
    var attacker = characters.FirstOrDefault(c => c.Id == req.AttackerId);
    var defender = characters.FirstOrDefault(c => c.Id == req.DefenderId);
    if (attacker is null || defender is null)
        return Results.BadRequest(new { error = "Invalid attacker or defender id" });

    // choose ability or basic attack
    RacMonsters.Server.Models.Ability? ability = null;
    if (req.AbilityIndex is int idx)
    {
        if (idx < 0 || idx >= attacker.Abilities.Length)
            return Results.BadRequest(new { error = "Invalid ability index" });
        ability = attacker.Abilities[idx];
    }

    var rng = Random.Shared;
    int attackStat = ability?.IsTech == true ? attacker.TechAttack : attacker.Attack;
    int defenseStat = ability?.IsTech == true ? defender.TechDefense : defender.Defense;

    // basic damage formula with randomness
    var basePower = ability?.Power ?? 8;
    var variance = rng.Next(-3, 4);
    var raw = Math.Max(1, basePower + attackStat - (int)(defenseStat * 0.6) + variance);

    // apply damage to defender
    defender.Health = Math.Max(0, defender.Health - raw);

    var msg = ability is null
        ? $"{attacker.Name} hits {defender.Name} for {raw} damage."
        : $"{attacker.Name} used {ability.Name} and dealt {raw} damage to {defender.Name}.";

    var result = new BattleResult(defender.Id, defender.Health, msg);
    return Results.Ok(result);
});

api.MapPost("battle/heal", (HealRequest req) =>
{
    var target = characters.FirstOrDefault(c => c.Id == req.TargetId);
    if (target is null)
        return Results.BadRequest(new { error = "Invalid target id" });

    var rng = Random.Shared;
    var heal = rng.Next(8, 16);
    target.Health = Math.Min(target.MaxHealth, target.Health + heal);
    var msg = $"{target.Name} healed for {heal} HP.";
    var res = new BattleResult(target.Id, target.Health, msg);
    return Results.Ok(res);
});

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
