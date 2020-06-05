namespace Cassandra.Fluent.Migrator.Core
{
    using System;

    public interface IMigrator
    {
        string Name { get; }

        Version Version { get; }

        string Description { get; }

        void ApplyMigration();
    }
}
