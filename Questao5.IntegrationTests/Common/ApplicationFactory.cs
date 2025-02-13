using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.IntegrationTests.Common;

public sealed class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public Task InitializeAsync() => Task.CompletedTask;

    Task IAsyncLifetime.DisposeAsync() => _connection.DisposeAsync().AsTask();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            _connection = new SqliteConnection("DataSource=:memory:;Mode=Memory;Cache=Shared");
            _connection.Open();

            var dbConfigServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DatabaseConfig));
            if (dbConfigServiceDescriptor != null)
                services.Remove(dbConfigServiceDescriptor);

            var dbBootstrpServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDatabaseBootstrap));
            if (dbBootstrpServiceDescriptor != null)
                services.Remove(dbBootstrpServiceDescriptor);

            services.AddSingleton(new DatabaseConfig { ConnectionString = _connection.ConnectionString });
            services.AddSingleton<IDatabaseBootstrap, TestDatabaseBootstrap>();
        });
    }
}