namespace Cassandra.Fluent.Migrator.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Core.Models;

    public interface ICassandraMigrator
    {
        /// <summary>
        /// Start the migration process.
        /// The method fetch the registred migrations from the {Services Provider} of the app.
        /// Before appling a migration, the method checks if already applied, If True, it skipps
        /// the migration otherwise applies it using the {ApplyMigration()} of the Migration.
        /// </summary>
        internal void Migrate();

        /// <summary>
        /// Get the latest migration that applied to the schema.
        /// </summary>
        ///
        /// <returns>Migration history details.</returns>
        Task<MigrationHistory> GetLatestMigrationAsync();

        /// <summary>
        /// Get the latest migration that applied to the schema.
        /// </summary>
        ///
        /// <returns>Migration history details.</returns>
        MigrationHistory GetLatestMigration();

        /// <summary>
        /// Gets the list of the registred migrations from the app {services provider}.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        Task<ICollection<MigrationHistory>> GetMigrationsAsync();

        /// <summary>
        /// Gets the list of the registred migrations from the app {services provider}.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        ICollection<IMigrator> GetMigrations();

        /// <summary>
        /// Gets the list of the applied migrations from the databse.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        Task<ICollection<MigrationHistory>> GetAppliedMigrationAsync();

        /// <summary>
        /// Gets the list of the applied migrations from the databse.
        /// </summary>
        ///
        /// <returns>List of migrations.</returns>
        ICollection<MigrationHistory> GetAppliedMigration();
    }
}
