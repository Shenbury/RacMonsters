using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RacMonsters.Server.Data;
using StackExchange.Redis;

namespace Microsoft.Extensions.Hosting
{
    public static class InfrastructureExtensions
    {
        public static TBuilder AddSqlServerAndRedis<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            var configuration = builder.Configuration;
            builder.AddSqlServerDbContext<RacMonstersDbContext>(connectionName: "rmserver", options => options.ConnectionString = configuration.GetConnectionString("rmdb"));

            // Register distributed cache backed by Redis
            builder.AddRedisClient(connectionName: "cache");

            return builder;
        }

        private class SqlServerHealthCheck : IHealthCheck
        {
            private readonly string _connectionString;

            public SqlServerHealthCheck(string connectionString)
            {
                _connectionString = connectionString;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    await using var conn = new SqlConnection(_connectionString);
                    await conn.OpenAsync(cancellationToken);
                    await conn.CloseAsync();
                    return HealthCheckResult.Healthy("SQL Server is reachable");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("SQL Server check failed", ex);
                }
            }
        }

        private class RedisHealthCheck : IHealthCheck
        {
            private readonly string _configuration;

            public RedisHealthCheck(string configuration)
            {
                _configuration = configuration;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    var opts = ConfigurationOptions.Parse(_configuration);
                    opts.AbortOnConnectFail = false;
                    await using var mux = await ConnectionMultiplexer.ConnectAsync(opts);
                    var db = mux.GetDatabase();
                    var pong = await db.PingAsync();
                    return HealthCheckResult.Healthy($"Redis responded in {pong.TotalMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Redis check failed", ex);
                }
            }
        }
    }
}
