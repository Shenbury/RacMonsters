var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var sql = builder.AddSqlServer("rmserver");
var sqldb = sql.AddDatabase("rmdb");

var server = builder.AddProject<Projects.RacMonsters_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(sqldb)
    .WithReference(sqldb); ;

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
