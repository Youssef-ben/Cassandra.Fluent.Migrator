namespace Cassandra.Fluent.Migrator.Common.Configuration.TestsServiceProvider
{
    using System;
    using System.Collections.Generic;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class InMemoryServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, object> services;

        public InMemoryServiceProvider()
        {
            var cassandraSession = this.GetTestCassandraSession($"migrations_75dca907_ac4e_433e_9b95");

            var migrationLogger = new Mock<ILogger<InitialMigration>>();
            var migratorLogger = new Mock<ILogger<CassandraMigrator>>();

            ICassandraFluentMigrator cfmHelper = new CassandraFluentMigrator(cassandraSession);
            IEnumerable<IMigrator> migration = new List<IMigrator>
            {
                new InitialMigration(migrationLogger.Object, cfmHelper),
            };

            this.services = new Dictionary<Type, object>
            {
                { typeof(ISession), cassandraSession },
                { typeof(ILogger<InitialMigration>), migrationLogger.Object },
                { typeof(ILogger<CassandraMigrator>), migratorLogger.Object },
                { typeof(ICassandraFluentMigrator), cfmHelper },
                { typeof(IEnumerable<IMigrator>), migration },
            };
        }

        public object GetService(Type serviceType)
        {
            this.services.TryGetValue(serviceType, out object value);

            if (value is null)
            {
                throw new ObjectNotFoundException($"The type [{serviceType.Name}] doesn't exists in the In Memory service provider!");
            }

            return value;
        }

        public TInterface GetTestService<TInterface>()
            where TInterface : class
        {
            return (TInterface)this.GetService(typeof(TInterface));
        }
    }
}
