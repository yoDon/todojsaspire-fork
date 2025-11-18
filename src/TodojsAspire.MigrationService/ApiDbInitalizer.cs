using System.ComponentModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using TodojsAspire.ApiService;

// based on: https://github.com/dotnet/aspire-samples/blob/main/samples/DatabaseMigrations/DatabaseMigrations.MigrationService/ApiDbInitializer.cs

namespace TodojsAspire.MigrationService;

public class ApiDbInitalizer(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime): BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
        }
        catch(Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        // stop this app
        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(TodoDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it doesn't exist
            // Do this first so there is a database to start the transaction against
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(TodoDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            try
            {
                Console.WriteLine("running migration");
                // Run migration in a transaction to avoid partial migration if it fails.
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                await dbContext.Database.MigrateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Console.WriteLine("Transaction committed");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

        });
    }
}