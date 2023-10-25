namespace Cassandra.Fluent.Migrator.Core
{
    using System.Collections.Generic;
    using Models;

    public interface ICassandraMigrator
    {
        /// <summary>
        ///     <para>
        ///         Start the migration process.
        ///     </para>
        ///     <para>
        ///         !! IMPORTANT NOTE - USE THIS METHOD WITH CAUTION!! <br />
        ///         It's recommended to apply the migration at the project startup using the following method as it
        ///         runs some additional checkups to make sure everything is set properly.
        ///         <see cref="CassandraFluentMigratorConfiguration.UseCassandraMigration" /> .
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This method uses the (ApplyMigration()) method of the each registered migration that was found,
        ///     but only apply the migration if it's version is bigger than the latest applied one. So make sure that
        ///     every new version that you create has it's version bigger than the last one.
        /// </remarks>
        /// <returns>The number of migrations that were applied during the startup.</returns>
        int Migrate();

        /// <summary>
        ///     Get the latest migration that was applied to the schema.
        /// </summary>
        /// <returns>The Migration details.</returns>
        MigrationHistory GetLatestMigration();

        /// <summary>
        ///     Gets the list of the registered migrations in the project service provider.
        ///     <para>
        ///         Note that the migrations are automatically sorted older to latest.
        ///     </para>
        /// </summary>
        /// <returns>List of migrations.</returns>
        ICollection<IMigrator> GetRegisteredMigrations();

        /// <summary>
        ///     Gets the list of the applied migrations from the database.
        ///     <para>
        ///         Note that the migrations are automatically sorted older to latest.
        ///     </para>
        /// </summary>
        /// <returns>List of migrations.</returns>
        ICollection<MigrationHistory> GetAppliedMigrations();
    }
}