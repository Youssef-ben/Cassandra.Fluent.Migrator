namespace Cassandra.Fluent.Migrator.Tests.Models;

using Mapping.Attributes;

public class CfmHelperObject
{
    [PartitionKey]
    public int Id { get; set; }

    public string Values { get; set; }

    public bool AddedColumnFromTestWithoutType { get; set; }
}