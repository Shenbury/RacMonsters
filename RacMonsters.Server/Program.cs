using RacMonsters.Server.Models;
using RacMonsters.Server.Services;
using RacMonsters.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register character loader service
builder.Services.AddSingleton<ICharacterService, CharacterService>();
builder.Services.AddSingleton<IBattleService, BattleService>();

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

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
