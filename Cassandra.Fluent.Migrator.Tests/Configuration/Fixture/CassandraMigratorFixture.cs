namespace Cassandra.Fluent.Migrator.Tests.Configuration.Fixture;

using System;
using Core;
using Helper;
using Microsoft.Extensions.Logging;

public sealed class CassandraMigratorFixture : IDisposable
{
    public readonly ICassandraMigrator Migrator;
    public readonly ICassandraFluentMigrator MigratorHelper;
    private ISession session;

    public CassandraMigratorFixture()
    {
        InMemoryServiceProvider serviceProvider = this.GetTestInMemoryProvider();
        var logger = serviceProvider.GetTestService<ILogger<CassandraMigrator>>();

        Migrator = new CassandraMigrator(serviceProvider, logger);
        MigratorHelper = serviceProvider.GetTestService<ICassandraFluentMigrator>();
        session = serviceProvider.GetTestService<ISession>();
    }

    public void Dispose()
    {
        Dispose(true);
    }

    public ISession GetSession(string keyspace = default)
    {
        return session ??= this.GetTestCassandraSession(keyspace);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (session is null)
        {
            return;
        }

        try
        {
            session.DeleteKeyspaceIfExists(session.Keyspace);
            session.ShutdownAsync().GetAwaiter().GetResult();
        }
        catch
        {
            // ignored
        }
    }
}