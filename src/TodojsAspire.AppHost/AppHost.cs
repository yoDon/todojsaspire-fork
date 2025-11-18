var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlite("db")
    .WithSqliteWeb();

var apiService = builder.AddProject<Projects.TodojsAspire_ApiService>("todoapiservice")
    .WithReference(db)
    .WithHttpHealthCheck("/health");

// use `aspire add javascript` for `AddViteApp`
var frontend = builder.AddViteApp("todofrontend", "../todo-frontend")
    .WithReference(apiService)
    .WaitFor(apiService);

apiService.PublishWithContainerFiles(frontend, "./wwwroot");

builder.Build().Run();
