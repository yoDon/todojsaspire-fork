var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlite("db")
    .WithSqliteWeb();

var apiService = builder.AddProject<Projects.TodojsAspire_ApiService>("apiservice")
    .WithReference(db)
    .WithHttpHealthCheck("/health");

// AddViteApp comes from community-toolkit
// use `aspire add node` and select 'ct-extensions'
builder.AddViteApp(name: "todo-frontend", workingDirectory: "../todo-frontend")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithNpmPackageInstallation();

builder.Build().Run();
