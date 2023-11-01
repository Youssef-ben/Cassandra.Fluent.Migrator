namespace Cassandra.Fluent.Migrator.Core.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Data.Linq;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;
    using Models;

    internal static class CassandraMigratorExtensions
    {
        internal static void UpdateMigrationHistory([NotNull] this ICassandraMigrator self,
                [NotNull] Table<MigrationHistory> table, [NotNull] IMigrator migration)
        {
            Check.NotNull(self, "the argument [Cassandra Migrator]");
            Check.NotNull(table, "the argument [Migration History]");
            Check.NotNull(migration, "the argument [Migration]");

            var result = new MigrationHistory
            {
                Name = migration.Name,
                Version = migration.Version.ToString(),
                CreatedAt = DateTime.UtcNow,
                Description = migration.Description
            };

            table
                    .Insert(result)
                    .Execute();
        }
    }
}