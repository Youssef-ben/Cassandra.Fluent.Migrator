namespace Cassandra.Fluent.Migrator.Example.Models.Domain
{
    using System;
    using Cassandra.Mapping.Attributes;

    public class Users
    {
        [PartitionKey]
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Language { get; set; }

        public Address Address { get; set; }
    }
}
