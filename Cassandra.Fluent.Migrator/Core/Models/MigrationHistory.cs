namespace Cassandra.Fluent.Migrator.Core.Models
{
    using System;
    using Cassandra.Mapping.Attributes;

    public class MigrationHistory
    {
        [PartitionKey]
        public string Name { get; set; }

        public string Version { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Description { get; set; }
    }
}
