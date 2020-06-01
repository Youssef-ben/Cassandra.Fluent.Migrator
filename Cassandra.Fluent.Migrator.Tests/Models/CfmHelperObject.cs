namespace Cassandra.Fluent.Migrator.Tests.Models
{
    using Cassandra.Mapping.Attributes;

    public class CfmHelperObject
    {
        [PartitionKey]
        public int Id { get; set; }

        public string Values { get; set; }

        public bool AddedColumnFromTestwithoutType { get; set; }
    }
}
