var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var db1 = sqlserver.AddDatabase("db1");

var migrationService = builder.AddProject<Projects.TodojsAspire_MigrationService>("migration")
    .WithReference(db1)
    .WaitFor(db1);

var apiService = builder.AddProject<Projects.TodojsAspire_ApiService>("todoapiservice")
    .WithReference(db1)
    .WaitForCompletion(migrationService)
    .WithHttpHealthCheck("/health");

// use `aspire add javascript` for `AddViteApp`
var frontend = builder.AddViteApp("todofrontend", "../todo-frontend")
    .WithReference(apiService)
    .WaitFor(apiService);

apiService.PublishWithContainerFiles(frontend, "./wwwroot");

builder.Build().Run();
