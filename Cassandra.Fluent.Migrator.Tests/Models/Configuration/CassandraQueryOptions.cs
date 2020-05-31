namespace Cassandra.Fluent.Migrator.Tests.Models.Configuration
{
    public class CassandraQueryOptions
    {
        public ConsistencyLevel? ConsistencyLevel { get; set; }

        public int HeartBeat { get; set; }

        public bool? TracingEnabled { get; set; }

        public int? PageSize { get; set; }
    }
}
