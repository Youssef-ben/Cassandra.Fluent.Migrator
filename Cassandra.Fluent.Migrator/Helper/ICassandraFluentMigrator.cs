namespace Cassandra.Fluent.Migrator.Helper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Data.Linq;
    using Utils.Exceptions;

    public interface ICassandraFluentMigrator
    {
        /// <summary>
        ///     Gets the registered Cassandra session context.
        /// </summary>
        /// <returns>Current instance of the Cassandra session.</returns>
        ISession GetCassandraSession();

        /// <summary>
        ///     Gets the Cassandra table based on the given (TEntity) class.
        /// </summary>
        /// <typeparam name="TEntity">The Class that represent a table in Cassandra.</typeparam>
        /// <returns>Instance of the Cassandra table.</returns>
        Table<TEntity> GetTable<TEntity>()
                where TEntity : class;

        /// <summary>
        ///     Validate that the table exists in the Cassandra session context.
        /// </summary>
        /// <param name="table">The table we need to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        bool DoesTableExists([NotNull] string table);

        /// <summary>
        ///     Validate that the column exists in the specified table.
        ///     <para>Before validating that the columns exists, the method first validate that the table exists.</para>
        /// </summary>
        /// <param name="table">The table we want to search.</param>
        /// <param name="column">The column that we want to search for.</param>
        /// <returns>True, if exists. False, Otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table isn't available in the current Cassandra session.</exception>
        bool DoesColumnExists([NotNull] string table, [NotNull] string column);

        /// <summary>
        ///     Validate that the User-Defined type exists in the Cassandra session context.
        /// </summary>
        /// <param name="udt">The User-Defined type that we need to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        bool DoesUdtExists([NotNull] string udt);

        /// <summary>
        ///     Validate that the column exists in the specified User-Defined type.
        ///     <para>Before validating that the columns exists, the method first validate that the User-Defined type exists.</para>
        /// </summary>
        /// <param name="udt">The User-Defined type we want to search.</param>
        /// <param name="column">The column that we want to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">
        ///     Thrown when the User-Defined type isn't available in the current Cassandra
        ///     session.
        /// </exception>
        bool DoesUdtColumnExists([NotNull] string udt, [NotNull] string column);

        /// <summary>
        ///     Validate that the Materialized view exists in the Cassandra session context.
        /// </summary>
        /// <param name="view">The Materialized view that we need to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        bool DoesMaterializedViewExists([NotNull] string view);

        /// <summary>
        ///     Validate that the column exists in the specified Materialized view.
        ///     <para>Before validating that the columns exists, the method first validate that the Materialized view exists.</para>
        /// </summary>
        /// <param name="view">The Materialized view. we want to search.</param>
        /// <param name="column">The column that we want to search for.</param>
        /// <returns>True, if exists. False otherwise.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">
        ///     Thrown when the Materialized view isn't available in the current Cassandra
        ///     session.
        /// </exception>
        bool DoesMaterializedViewColumnExists([NotNull] string view, [NotNull] string column);

        /// <summary>
        ///     Get the Cassandra CQL type equivalent to the specified CSharp type.
        /// </summary>
        /// <param name="type">The CSharp type to be converted.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>Return string value containing the Cassandra CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        string GetCqlType([NotNull] Type type, bool shouldBeFrozen = false);

        /// <summary>
        ///     Get the Cassandra CQL type of the specified column.
        /// </summary>
        /// <typeparam name="TEntity">The class where we need to search for the column type.</typeparam>
        /// <param name="column">The column that we want to search for.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>Return string value of the Cassandra CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        string GetColumnType<TEntity>([NotNull] string column, bool shouldBeFrozen = false)
                where TEntity : class;
    }
}