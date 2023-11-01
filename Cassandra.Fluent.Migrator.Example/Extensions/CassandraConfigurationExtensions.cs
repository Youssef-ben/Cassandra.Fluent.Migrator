namespace Cassandra.Fluent.Migrator.Example.Extensions
{
    using System;
    using Common.Models.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class CassandraConfigurationExtensions
    {
        public static IServiceCollection AddCassandraSession(this IServiceCollection self, IConfiguration configuration)
        {
            (IConfigurationSection configurationSection, CassandraSettings cassandraSettings) =
                    configuration.LoadCassandraSettings();

            ISession cassandraSession = cassandraSettings.BuildClusterAndConnect("net7");

            return self
                    .Configure<CassandraSettings>(configurationSection)
                    .AddTransient(opt => cassandraSession);
        }

        private static (IConfigurationSection, CassandraSettings) LoadCassandraSettings(
                this IConfiguration configuration)
        {
            IConfigurationSection configurationSection = configuration.GetSection("Cassandra");

            var cassandraSettings = new CassandraSettings();
            configurationSection.Bind(cassandraSettings);
            return (configurationSection, cassandraSettings);
        }

        private static ISession BuildClusterAndConnect(this CassandraSettings self, string keyspace)
        {
            var username = self.Credentials.Username;
            var password = self.Credentials.Password;
            keyspace = string.IsNullOrWhiteSpace(keyspace) ? self.DefaultKeyspace : keyspace;

            PoolingOptions heartbeat = new PoolingOptions().SetHeartBeatInterval(self.Query.HeartBeat);
            QueryOptions consistencyQueryOption = default;
            if (self.Query.ConsistencyLevel.HasValue)
            {
                consistencyQueryOption = new QueryOptions()
                        .SetConsistencyLevel(self.Query.ConsistencyLevel.Value);
            }

            if (string.Equals(self.Replication["class"], "SimpleStrategy", StringComparison.CurrentCultureIgnoreCase))
            {
                self.Replication.Remove("datacenter");
            }
            else if (string.Equals(
                    self.Replication["class"],
                    "NetworkTopologyStrategy",
                    StringComparison.CurrentCultureIgnoreCase))
            {
                self.Replication.Remove("replication_factor");
            }

            return Cluster.Builder()
                    .AddContactPoints(self.ContactPoints)
                    .WithPort(self.Port)
                    .WithCredentials(username, password)
                    .WithCompression(CompressionType.Snappy)
                    .WithQueryOptions(consistencyQueryOption)
                    .WithLoadBalancingPolicy(new TokenAwarePolicy(new DCAwareRoundRobinPolicy()))
                    .WithPoolingOptions(heartbeat)
                    .WithDefaultKeyspace(keyspace)
                    .Build()
                    .ConnectAndCreateDefaultKeyspaceIfNotExists(self.Replication);
        }
    }
}