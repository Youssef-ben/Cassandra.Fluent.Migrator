namespace Cassandra.Fluent.Migrator.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Cassandra.Data.Linq;
    using Cassandra.Fluent.Migrator.Core.Models;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class CassandraMigratorExtensions
    {
        internal static MigrationHistory UpdateMigrationHistory([NotNull]this ICassandraMigrator self, [NotNull] Table<MigrationHistory> table, [NotNull]IMigrator migration)
        {
            Check.NotNull(self, $"the argument [Cassandra Migrator]");
            Check.NotNull(table, $"the argument [Migration History]");
            Check.NotNull(migration, $"the argument [Migration]");

            var result = new MigrationHistory
            {
                Name = migration.Name,
                Version = migration.Version.ToString(),
                CreatedAt = DateTime.UtcNow,
                Description = migration.Description,
            };

            table
                .Insert(result)
                .Execute();

            return result;
        }

        internal static bool ShouldApplyMigration([NotNull]this ICassandraMigrator self, [NotNull]IMigrator migration)
        {
            Check.NotNull(self, $"the argument [Cassandra Migrator]");
            Check.NotNull(migration, $"the argument [Migration]");

            var latestAppliedMigration = self.GetLatestMigration();

            if (latestAppliedMigration is null || migration.Version > new Version(latestAppliedMigration.Version))
            {
                return true;
            }

            return false;
        }
    }
}
