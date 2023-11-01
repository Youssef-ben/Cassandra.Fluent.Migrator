namespace Cassandra.Fluent.Migrator.Common.Models.Configuration;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CassandraCredentials
{
    public string Username { get; set; }

    public string Password { get; set; }
}