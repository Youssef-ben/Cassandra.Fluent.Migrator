namespace Cassandra.Fluent.Migrator.Common.Models.Configuration;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CassandraReplication
{
    public string Class { get; set; }

    public string Datacenter { get; set; }
}