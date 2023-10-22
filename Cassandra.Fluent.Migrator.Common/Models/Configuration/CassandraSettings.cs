namespace Cassandra.Fluent.Migrator.Common.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CassandraSettings
{
    public ICollection<string> ContactPoints { get; set; }

    public CassandraCredentials Credentials { get; set; }

    public int Port { get; set; }

    public string DefaultKeyspace { get; set; }

    public Dictionary<string, string> Replication { get; set; }

    public CassandraQueryOptions Query { get; set; }

    public bool DurableWrites { get; set; } = true;

    public CassandraSettings ValidateSetting()
    {
        const string ARGUMENT_NULL_EXCEPTION_MESSAGE = "The configuration [{0}] section is invalid";

        if (ContactPoints is null || ContactPoints.Count == 0)
        {
            throw new ArgumentNullException(string.Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Contact Point"));
        }

        if (Port == 0)
        {
            throw new ArgumentNullException(string.Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Port"));
        }

        if (Credentials is null || string.IsNullOrWhiteSpace(Credentials.Username) ||
            string.IsNullOrWhiteSpace(Credentials.Password))
        {
            throw new ArgumentNullException(string.Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Credentials"));
        }

        if (string.IsNullOrWhiteSpace(DefaultKeyspace))
        {
            throw new ArgumentNullException(string.Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Default Keyspace"));
        }

        if (Replication is null || string.IsNullOrWhiteSpace(Replication["class"]))
        {
            throw new ArgumentNullException(string.Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Replication"));
        }

        if (Replication["class"].ToLower() == "NetworkTopologyStrategy".ToLower() &&
            string.IsNullOrWhiteSpace(Replication["datacenter"]))
        {
            throw new ArgumentNullException(string
                    .Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Replication: datacenter"));
        }

        if (Replication["class"].ToLower() == "SimpleStrategy".ToLower() &&
            string.IsNullOrWhiteSpace(Replication["replication_factor"]))
        {
            throw new ArgumentNullException(string
                    .Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Replication: replication_factor"));
        }

        if (Query is null || Query.HeartBeat == 0)
        {
            throw new ArgumentNullException(string.Format(ARGUMENT_NULL_EXCEPTION_MESSAGE, "Query"));
        }

        return this;
    }
}