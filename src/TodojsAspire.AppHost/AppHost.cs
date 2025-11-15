var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlite("db")
    .WithSqliteWeb();

var apiService = builder.AddProject<Projects.TodojsAspire_ApiService>("apiservice")
    .WithReference(db)
    .WithHttpHealthCheck("/health");

// use `aspire add javascript` for `AddViteApp`
builder.AddViteApp("todo-frontend", "../todo-frontend")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
