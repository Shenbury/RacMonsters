using RacMonsters.Server.Models;
using RacMonsters.Server.Services;
using RacMonsters.Server.Repositories.Characters;
using RacMonsters.Server.Repositories.Abilities;
using RacMonsters.Server.Repositories.Battles;
using RacMonsters.Server.Repositories.Sessions;
using RacMonsters.Server.Repositories.Rounds;
using RacMonsters.Server.Services.Characters;
using RacMonsters.Server.Services.Abilities;
using RacMonsters.Server.Services.Battles;
using RacMonsters.Server.Services.Sessions;
using RacMonsters.Server.Services.Rounds;
using RacMonsters.Server.Endpoints;
using RacMonsters.Server.Services.AIs;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register character loader service
// repositories
builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();
builder.Services.AddSingleton<IAbilityRepository, AbilityRepository>();
builder.Services.AddSingleton<IBattleRepository, BattleRepository>();
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IRoundRepository, RoundRepository>();

// services
builder.Services.AddSingleton<ICharacterService, CharacterService>();
builder.Services.AddSingleton<IAbilityService, AbilityService>();
builder.Services.AddSingleton<IBattleService, BattleService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IRoundService, RoundService>();
builder.Services.AddSingleton<IAIService, AIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var api = app.MapGroup("/api");

// register endpoints in extension classes
api.MapCharacterEndpoints();
api.MapBattleEndpoints();
api.MapAbilityEndpoints();
api.MapSessionEndpoints();
api.MapRoundEndpoints();

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
