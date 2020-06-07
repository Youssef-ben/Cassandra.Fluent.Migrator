namespace Cassandra.Fluent.Migrator.Helper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Cassandra.Data.Linq;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;

    public interface ICassandraFluentMigrator
    {
        /// <summary>
        /// Gets the registred Cassandra session context.
        /// </summary>
        /// <returns>Current instance of the Cassadnra session.</returns>
        ISession GetCassandraSession();

        /// <summary>
        /// Gets the Cassandra table based on the {TEntity}.
        /// </summary>
        /// <typeparam name="TEntity">The Class that represent a table in Cassandra.</typeparam>
        /// <returns>Instance of the Cassandra table.</returns>
        Table<TEntity> GetTable<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Checks if the table exists in the Cassandra session context.
        /// </summary>
        /// <param name="table">The table we need to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when the table value is null or empty.</exception>
        bool DoesTableExists([NotNull]string table);

        /// <summary>
        /// Checks if the column exits in the specified table.
        /// </summary>
        ///
        /// <remarks>
        /// Before checking that the columns exists, the method first checks that the table exists.
        /// </remarks>
        ///
        /// <param name="table">The table we want to search.</param>
        /// <param name="column">The column that we want to search for.</param>
        /// <returns>True, if exists. False, Otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when the table or column value is null or empty.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table isn't available in the current casasndra session.</exception>
        bool DoesColumnExists([NotNull]string table, [NotNull]string column);

        /// <summary>
        /// Checks if the User-Defined type exists in the Cassandra session context.
        /// </summary>
        /// <param name="udt">The User-Defined type that we need to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when the User-Defined type value is null or empty.</exception>
        bool DoesUdtExists([NotNull]string udt);

        /// <summary>
        /// Checks if the column exits in the specified User-Defined type.
        /// </summary>
        ///
        /// <remarks>
        /// Before checking that the columns exists, the method first checks that the User-Defined type exists.
        /// </remarks>
        ///
        /// <param name="udt">The User-Defined type we want to search.</param>
        /// <param name="column">The column that we want to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when the User-Defined type or column value is null or empty.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the User-Defined type isn't available in the current casasndra session.</exception>
        bool DoesUdtColumnExists([NotNull]string udt, [NotNull]string column);

        /// <summary>
        /// Checks if the Materialized view exists in the Cassandra session context.
        /// </summary>
        /// <param name="view">The Materialized view that we need to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when the Materialized view value is null or empty.</exception>
        bool DoesMaterializedViewExists([NotNull]string view);

        /// <summary>
        /// Checks if the column exits in the specified Materialized view.
        /// </summary>
        ///
        /// <remarks>
        /// Before checking that the columns exists, the method first checks that the Materialized view exists.
        /// </remarks>
        ///
        /// <param name="view">The Materialized view. we want to search.</param>
        /// <param name="column">The column that we want to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when the Materialized view or the column value is null or empty.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the Materialized view isn't available in the current casasndra session.</exception>
        bool DoesMaterializedViewColumnExists([NotNull]string view, [NotNull] string column);

        /// <summary>
        /// Get the Cassandra CQL type equivalent to the specified CSharp type.
        /// </summary>
        /// <param name="type">The CSharp type to be converted.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>Return string value containing the Cassandra CQL type.</returns>
        string GetCqlType([NotNull]Type type, bool shouldBeFrozen = false);

        /// <summary>
        /// Get the Cassandra CQL type of the specified column.
        /// </summary>
        /// <typeparam name="TEntity">The class where we need to look for the column type.</typeparam>
        /// <param name="column">The column that we want to search for.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>Return string value of the Cassandra CQL type.</returns>
        string GetColumnType<TEntity>([NotNull]string column, bool shouldBeFrozen = false)
            where TEntity : class;
    }
}
