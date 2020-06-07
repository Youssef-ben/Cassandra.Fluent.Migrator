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
        /// The method fetch the registred migrations from the {Services Provider} of the app.
        /// Before appling a migration, the method checks if its already applied, If True, it skipps
        /// the migration otherwise applies it using the {ApplyMigration()} of the current migration.
        ///
        /// <para>NOTE: Use with caution.
        /// It's recommended to apply the migration at the project startup using the
        /// <see cref="CassandraFluentMigratorConfiguration.UseCassandraMigration(IApplicationBuilder)"/> method.</para>
        /// </summary>
        ///
        /// <returns>Count of the applied migrations.</returns>
        int Migrate();

        /// <summary>
        /// Get the latest migration that was applied to the schema.
        /// </summary>
        ///
        /// <returns>Migration history details.</returns>
        MigrationHistory GetLatestMigration();

        /// <summary>
        /// Gets the list of the registred migrations from the app {services provider}.
        /// The migrations are automatically sorted older to latest.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        ICollection<IMigrator> GetRegistredMigrations();

        /// <summary>
        /// Gets the list of the applied migrations from the databse.
        /// The migrations are automatically sorted latest to older.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        ICollection<MigrationHistory> GetAppliedMigrations();
    }
}
