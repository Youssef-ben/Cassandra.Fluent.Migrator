namespace Cassandra.Fluent.Migrator.Tests.Configuration
{
    using System.Diagnostics.CodeAnalysis;
    using Cassandra.Fluent.Migrator.Tests.Models.Configuration;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    public static class CasssandraExtensions
    {
        /// <summary>
        /// Validate the Cassandra Settings, Create a new connection to the database and return the newly created session.
        /// </summary>
        /// <typeparam name="TClass">The type of the class calling his method.</typeparam>
        /// <param name="self">The class calling this method.</param>
        /// <param name="keyspace">The Database name space.</param>
        /// <returns>Return a new Session instance.</returns>
        public static ISession GetCassandraSession<TClass>([NotNull]this TClass self, string keyspace = default)
            where TClass : class
        {
            Check.NotNull(self);

            return SettingsExtensions
                .GetConfiguration()
                .BuildClusterAndConnect(keyspace);
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
               .WithDefaultKeyspace(string.IsNullOrWhiteSpace(keyspace) ? self.DefaultKeyspace : keyspace)
               .Build()
               .ConnectAndCreateDefaultKeyspaceIfNotExists(self.Replication);
        }
    }
}
