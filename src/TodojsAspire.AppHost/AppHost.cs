var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("todosqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var tododb = sqlserver.AddDatabase("tododb");

var migrationService = builder.AddProject<Projects.TodojsAspire_MigrationService>("migration")
    .WithReference(tododb)
    .WaitFor(tododb);

var apiService = builder.AddProject<Projects.TodojsAspire_ApiService>("todoapiservice")
    .WithReference(tododb)
    .WaitForCompletion(migrationService)
    .WithHttpHealthCheck("/health");

// use `aspire add javascript` for `AddViteApp`
var frontend = builder.AddViteApp("todofrontend", "../todo-frontend")
    .WithReference(apiService)
    .WaitFor(apiService);

apiService.PublishWithContainerFiles(frontend, "./wwwroot");

builder.Build().Run();
