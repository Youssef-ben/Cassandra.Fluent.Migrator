namespace Cassandra.Fluent.Migrator.Common.Configuration
{
    using System.Diagnostics.CodeAnalysis;
    using Cassandra.Fluent.Migrator.Common.Configuration.TestsServiceProvider;
    using Cassandra.Fluent.Migrator.Common.Models.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    public static class CassandraExtensions
    {
        /// <summary>
        /// Validate the Cassandra Settings, Create a new connection to the database and add them to the Service Collection.
        /// </summary>
        /// <param name="self">The Service collection calling this method.</param>
        /// <param name="configuration">The Database name space.</param>
        /// <returns>Return a new Session instance.</returns>
        public static IServiceCollection AddCassandraSession([NotNull]this IServiceCollection self, [NotNull]IConfiguration configuration)
        {
            Check.NotNull(self, $"The argument [Service Collection]");
            Check.NotNull(self, $"The argument [Configuration]");

            return self
                .Configure<CassandraSettings>(opt => configuration.GetCassandraSettings())
                .AddTransient<ISession>(opt => configuration.GetCassandraSettings().BuildClusterAndConnect());
        }

        /// <summary>
        /// Validate the Cassandra Settings, Create a new connection to the database and return the newly created session.
        /// </summary>
        /// <typeparam name="TClass">The type of the class calling his method.</typeparam>
        /// <param name="self">The class calling this method.</param>
        /// <param name="keyspace">The Database name space.</param>
        /// <returns>Return a new Session instance.</returns>
        public static ISession GetTestCassandraSession<TClass>([NotNull]this TClass self, string keyspace = default)
            where TClass : class
        {
            Check.NotNull(self);

            return SettingsExtensions
                .GetCassandraSettings()
                .BuildClusterAndConnect(keyspace);
        }

        /// <summary>
        /// Sets the test in memory Service provider and returns a new instance of the Cassandra Migrator.
        /// </summary>
        /// <typeparam name="TClass">The Class calling this method.</typeparam>
        /// <param name="self">The class calling this method.</param>
        /// <returns>Return a new Instance of the Cassandra Migrator.</returns>
        public static InMemoryServiceProvider GetTestInMemoryProvider<TClass>([NotNull]this TClass self)
            where TClass : class
        {
            Check.NotNull(self, "The argument [TClass]");

            return new InMemoryServiceProvider();
        }

        /// <summary>
        /// Setup and build the cassandra Cluster.
        /// </summary>
        /// <param name="self">The Cassandra settings.</param>
        /// <param name="keyspace">The Database name space.</param>
        /// <returns>Cassandra Cluster.</returns>
        private static ISession BuildClusterAndConnect(this CassandraSettings self, string keyspace = default)
        {
            var username = self.Credentials.Username;
            var password = self.Credentials.Password;
            keyspace = string.IsNullOrWhiteSpace(keyspace) ? self.DefaultKeyspace : keyspace;

            var consistentyQueryOption = new QueryOptions().SetConsistencyLevel((ConsistencyLevel)self.Query.ConsistencyLevel);
            var heartbeat = new PoolingOptions().SetHeartBeatInterval(self.Query.HeartBeat);

            if (self.Replication["class"].ToLower() == "SimpleStrategy".ToLower())
            {
                self.Replication.Remove("datacenter");
            }
            else if (self.Replication["class"].ToLower() == "NetworkTopologyStrategy".ToLower())
            {
                self.Replication.Remove("replication_factor");
            }

            return Cluster.Builder()
               .AddContactPoints(self.ContactPoints)
               .WithPort(self.Port)
               .WithCredentials(username, password)
               .WithCompression(CompressionType.Snappy)
               .WithQueryOptions(consistentyQueryOption)
               .WithLoadBalancingPolicy(new TokenAwarePolicy(new DCAwareRoundRobinPolicy()))
               .WithPoolingOptions(heartbeat)
               .WithDefaultKeyspace(keyspace)
               .Build()
               .ConnectAndCreateDefaultKeyspaceIfNotExists(self.Replication);
        }
    }
}
