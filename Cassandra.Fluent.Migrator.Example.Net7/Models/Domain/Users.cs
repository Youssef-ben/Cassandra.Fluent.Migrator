namespace Cassandra.Fluent.Migrator.Example.Net7.Models.Domain;

using Mapping.Attributes;

public class Users
{
    [PartitionKey]
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    [Frozen]
    public IEnumerable<Address> Address { get; set; } = new List<Address>();
}