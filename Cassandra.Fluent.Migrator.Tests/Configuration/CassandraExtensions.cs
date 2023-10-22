namespace Cassandra.Fluent.Migrator.Tests.Configuration;

using System;
using System.Diagnostics.CodeAnalysis;
using Common.Models.Configuration;
using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

public static class CassandraExtensions
{
    /// <summary>
    ///     Validate the Cassandra Settings, Create a new connection to the database and return the newly created session.
    /// </summary>
    /// <typeparam name="TClass">The type of the class calling his method.</typeparam>
    /// <param name="self">The class calling this method.</param>
    /// <param name="keyspace">The Database name space.</param>
    /// <returns>Return a new Session instance.</returns>
    public static ISession GetTestCassandraSession<TClass>([NotNull] this TClass self, string keyspace = default)
            where TClass : class
    {
        Check.NotNull(self);

        return SettingsExtensions
                .GetCassandraSettings()
                .BuildClusterAndConnect(keyspace);
    }

    /// <summary>
    ///     Sets the test in memory Service provider and returns a new instance of the Cassandra Migrator.
    /// </summary>
    /// <typeparam name="TClass">The Class calling this method.</typeparam>
    /// <param name="self">The class calling this method.</param>
    /// <returns>Return a new Instance of the Cassandra Migrator.</returns>
    public static InMemoryServiceProvider GetTestInMemoryProvider<TClass>([NotNull] this TClass self)
            where TClass : class
    {
        Check.NotNull(self, "The argument [TClass]");

        return new InMemoryServiceProvider();
    }

    /// <summary>
    ///     Setup and build the cassandra Cluster.
    /// </summary>
    /// <param name="self">The Cassandra settings.</param>
    /// <param name="keyspace">The Database name space.</param>
    /// <returns>Cassandra Cluster.</returns>
    private static ISession BuildClusterAndConnect(this CassandraSettings self, string keyspace = default)
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