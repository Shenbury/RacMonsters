using Microsoft.EntityFrameworkCore;
using System.Linq;
using RacMonsters.Server.Data;
using RacMonsters.Server.Repositories.Characters;
using RacMonsters.Server.Repositories.Abilities;
using RacMonsters.Server.Repositories.Battles;
using RacMonsters.Server.Repositories.Sessions;
using RacMonsters.Server.Repositories.Rounds;
using RacMonsters.Server.Services.Characters;
using RacMonsters.Server.Services.Abilities;
using RacMonsters.Server.Services.Battles;
using RacMonsters.Server.Services.Sessions;
using RacMonsters.Server.Repositories.Leaderboard;
using RacMonsters.Server.Services.Leaderboard;
using RacMonsters.Server.Services.Rounds;
using RacMonsters.Server.Endpoints;
using RacMonsters.Server.Services.AIs;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
// Configure infrastructure: SQL Server (EF Core) and Redis (distributed cache + health checks)
builder.AddSqlServerAndRedis();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register character loader service
// repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IAbilityRepository, AbilityRepository>();
builder.Services.AddScoped<IBattleRepository, BattleRepository>();
// leaderboard
builder.Services.AddScoped<RacMonsters.Server.Repositories.Leaderboard.ILeaderboardRepository, RacMonsters.Server.Repositories.Leaderboard.SqlLeaderboardRepository>();
builder.Services.AddScoped<RacMonsters.Server.Services.Leaderboard.ILeaderboardService, RacMonsters.Server.Services.Leaderboard.LeaderboardService>();

// register SQL-backed session repository
builder.Services.AddScoped<ISessionRepository, RacMonsters.Server.Repositories.Sessions.SqlSessionRepository>();
builder.Services.AddScoped<IRoundRepository, RoundRepository>();

// seeder
builder.Services.AddScoped<RacMonsters.Server.Data.DatabaseSeeder>();

// services
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAbilityService, AbilityService>();
builder.Services.AddScoped<IBattleService, BattleService>();
// session service is scoped because it depends on a scoped repository/DbContext
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IRoundService, RoundService>();
builder.Services.AddScoped<IAIService, AIService>();

var app = builder.Build();

// ensure DB created for sessions and run seeding
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<RacMonstersDbContext>();
        logger.LogInformation("Applying database migrations...");
        // Ensure database is created and apply migrations
        db.Database.EnsureCreated(); // Creates DB if it doesn't exist
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");

        var seeder = scope.ServiceProvider.GetRequiredService<RacMonsters.Server.Data.DatabaseSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw;
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Server-side session middleware: ensure each visitor receives a unique session id
app.Use(async (context, next) =>
{
    var sessionService = context.RequestServices.GetRequiredService<RacMonsters.Server.Services.Sessions.ISessionService>();
    const string cookieName = "rac_session";
    RacMonsters.Server.Models.Session? session = null;

    if (context.Request.Cookies.TryGetValue(cookieName, out var cookieVal) && int.TryParse(cookieVal, out var sessionId))
    {
        session = await sessionService.GetSession(sessionId);
    }

    if (session == null)
    {
        // create a new server-side session and issue cookie
        session = await sessionService.CreateSession(new RacMonsters.Server.Models.Session());
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        context.Response.Cookies.Append(cookieName, session.Id.ToString(), cookieOptions);
    }

    // expose session to downstream handlers if they want it
    context.Items["Session"] = session;

    await next();
});

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
api.MapLeaderboardEndpoints();

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
