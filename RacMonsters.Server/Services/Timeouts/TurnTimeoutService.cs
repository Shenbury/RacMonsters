using Microsoft.AspNetCore.SignalR;
using RacMonsters.Server.Hubs;
using RacMonsters.Server.Services.Battles;

namespace RacMonsters.Server.Services.Timeouts
{
    public class TurnTimeoutService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TurnTimeoutService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

        public TurnTimeoutService(IServiceProvider services, ILogger<TurnTimeoutService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Turn Timeout Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForExpiredTurns(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking for expired turns");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Turn Timeout Service stopped");
        }

        private async Task CheckForExpiredTurns(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var battleService = scope.ServiceProvider.GetRequiredService<IBattleService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();

            var expiredBattles = await battleService.GetBattlesWithExpiredTurns();

            if (expiredBattles.Count > 0)
            {
                _logger.LogInformation($"Found {expiredBattles.Count} battle(s) with expired turns");
            }

            foreach (var battle in expiredBattles)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    _logger.LogWarning($"Processing timeout for battle {battle.Id}");

                    // Process auto-move (random ability selection)
                    await battleService.ProcessAutoMove(battle.Id);

                    // Notify players about the timeout
                    await hubContext.Clients.Group($"battle-{battle.Id}")
                        .SendAsync("TurnTimeout", "Turn timeout - random move was selected automatically", stoppingToken);

                    _logger.LogInformation($"Auto-move processed for battle {battle.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing timeout for battle {battle.Id}");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Turn Timeout Service is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}
