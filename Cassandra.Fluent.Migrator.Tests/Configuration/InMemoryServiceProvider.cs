namespace Cassandra.Fluent.Migrator.Tests.Configuration;

using System;
using System.Collections.Generic;
using Core;
using Helper;
using Microsoft.Extensions.Logging;
using Migrations;
using Moq;
using Utils.Exceptions;

public class InMemoryServiceProvider : IServiceProvider
{
    private readonly IDictionary<Type, object> services;

    public InMemoryServiceProvider()
    {
        var keyspace = Guid.NewGuid().ToString()[..20].Replace("-", "_");
        ISession cassandraSession = this.GetTestCassandraSession(keyspace);

        var migrationLogger = new Mock<ILogger<InitialMigration>>();
        var migratorLogger = new Mock<ILogger<CassandraMigrator>>();

        ICassandraFluentMigrator cfmHelper = new CassandraFluentMigrator(cassandraSession);
        IEnumerable<IMigrator> migration = new List<IMigrator>
        {
            new InitialMigration(migrationLogger.Object, cfmHelper)
        };

        services = new Dictionary<Type, object>
        {
            { typeof(ISession), cassandraSession },
            { typeof(ILogger<InitialMigration>), migrationLogger.Object },
            { typeof(ILogger<CassandraMigrator>), migratorLogger.Object },
            { typeof(ICassandraFluentMigrator), cfmHelper },
            { typeof(IEnumerable<IMigrator>), migration }
        };
    }

    public object GetService(Type serviceType)
    {
        services.TryGetValue(serviceType, out var value);

        if (value is null)
        {
            throw new ObjectNotFoundException(
                    $"The type [{serviceType.Name}] doesn't exists in the In Memory service provider!");
        }

        return value;
    }

    public TInterface GetTestService<TInterface>()
            where TInterface : class
    {
        return (TInterface)GetService(typeof(TInterface));
    }
}