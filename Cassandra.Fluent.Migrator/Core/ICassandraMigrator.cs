namespace Cassandra.Fluent.Migrator.Core
{
    using System.Collections.Generic;
    using Cassandra.Fluent.Migrator.Core.Models;
    using Microsoft.AspNetCore.Builder;

    public interface ICassandraMigrator
    {
        /// <summary>
        /// Start the migration process.
        ///
        /// <para>
        ///     The method fetch the registered migrations from the {Services Provider} of the app and applies them.
        ///     Before running a migration, the method validate that it's not already applied, otherwise it skipps migration.
        ///     The migrator uses the {ApplyMigration()} method of the current migration.
        /// </para>
        ///
        /// <para>
        ///     NOTE: Use with caution.
        ///     It's recommended to apply the migration at the project startup using the
        ///     <see cref="CassandraFluentMigratorConfiguration.UseCassandraMigration(IApplicationBuilder)"/> method.
        /// </para>
        /// </summary>
        ///
        /// <returns>Count of the applied migrations.</returns>
        int Migrate();

        /// <summary>
        /// Get the latest migration that was applied to the schema.
        /// </summary>
        ///
        /// <returns>The Migration details.</returns>
        MigrationHistory GetLatestMigration();

        /// <summary>
        /// Gets the list of the registered migrations in the app {services provider}.
        /// The migrations are automatically sorted older to latest.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        ICollection<IMigrator> GetRegisteredMigrations();

        /// <summary>
        /// Gets the list of the applied migrations from the database.
        /// The migrations are automatically sorted latest to older.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        ICollection<MigrationHistory> GetAppliedMigrations();
    }
}
