namespace Cassandra.Fluent.Migrator.Tests.Models.Configuration
{
    using System.Collections.Generic;

    public class CassandraSettings
    {
        public List<string> ContactPoints { get; set; }

        public CassandraCredentials Credentials { get; set; }

        public int Port { get; set; }

        public string DefaultKeyspace { get; set; }

        public Dictionary<string, string> Replication { get; set; }

        public CassandraQueryOptions Query { get; set; }

        public bool DurableWrites { get; set; } = true;
    }
}
