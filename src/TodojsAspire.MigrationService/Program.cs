using Microsoft.EntityFrameworkCore;
using TodojsAspire.ApiService;
using TodojsAspire.MigrationService;

// based on: https://github.com/dotnet/aspire-samples/blob/main/samples/DatabaseMigrations/DatabaseMigrations.MigrationService/AppHost.cs

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ApiDbInitalizer>();

// Common Aspire service defaults (telemetry, health, discovery)
builder.AddServiceDefaults();

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db1"), sqlOptions =>
        sqlOptions.MigrationsAssembly("TodojsAspire.MigrationService")
    ));

// Optional: add enrichment (instrumentation) now that we're not using pooling.
builder.EnrichSqlServerDbContext<TodoDbContext>();

var app = builder.Build();

app.Run();

