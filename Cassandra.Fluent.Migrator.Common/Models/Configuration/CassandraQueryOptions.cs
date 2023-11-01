namespace Cassandra.Fluent.Migrator.Common.Models.Configuration;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CassandraQueryOptions
{
    public ConsistencyLevel? ConsistencyLevel { get; set; }

    public int HeartBeat { get; set; }

    public bool? TracingEnabled { get; set; }

    public int? PageSize { get; set; }
}