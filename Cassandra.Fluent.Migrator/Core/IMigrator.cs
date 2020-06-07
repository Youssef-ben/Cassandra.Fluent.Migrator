namespace Cassandra.Fluent.Migrator.Core
{
    using System;
    using System.Threading.Tasks;

    public interface IMigrator
    {
        string Name { get; }

        Version Version { get; }

        string Description { get; }

        Task ApplyMigrationAsync();
    }
}
