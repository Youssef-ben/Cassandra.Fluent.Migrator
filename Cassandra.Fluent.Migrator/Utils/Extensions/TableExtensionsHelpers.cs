namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Utils.Constants;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class TableExtensionsHelpers
    {
        /// <summary>
        /// Build and execute the Add column query statement.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="column">The column that we want to add.</param>
        /// <param name="type">The type of the columns.</param>
        /// <returns>Cassandra Fluent Migrator.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteCreateColumnQueryAsync([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull]string column, [NotNull] string type)
        {
            Check.NotNull(self, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(type)}]");

            var query = TableCqlStatements.TABLE_ADD_COLUMN_STATEMENT.NormalizeString(table, column, type);

            return await self.ExecuteStatementAsync(query, AppErrorsMessages.COLUMN_EXISTS.NormalizeString(column));
        }

        /// <summary>
        /// Build and execute the Rename column query statement.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="column">Old Column to be renamed.</param>
        /// <param name="target">New Column name.</param>
        /// <returns>Cassandra Fluent Migrator.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteRenameColumnQueryAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string table, [NotNull]string column, [NotNull]string target)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [old name]");
            Check.NotEmptyNotNull(target, $"The argument [new name]");

            var query = TableCqlStatements.TABLE_RENAME_COLUMN_STATEMENT.NormalizeString(table, column, target);

            return await self.ExecuteStatementAsync(query, AppErrorsMessages.COLUMN_EXISTS_FOR_RENAME.NormalizeString(column, target, self.GetCassandraSession().Keyspace));
        }

        /// <summary>
        /// Build and execute the Drop column query statement.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="column">The Column to be deleted.</param>
        /// <returns>Cassandra Fluent Migrator.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteDropColumnQueryAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string table, [NotNull]string column)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [{nameof(table)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            var query = TableCqlStatements.TABLE_DROP_COLUMN_STATEMENT.NormalizeString(table, column);

            return await self.ExecuteStatementAsync(query, AppErrorsMessages.COLUMN_NOT_FOUND.NormalizeString(column, table));
        }

        /// <summary>
        /// Check if the specified column is a [PrimaryKey].
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="column">The Column to check.</param>
        /// <returns>True if Primary, False Otherwise.</returns>
        ///
        /// <exception cref="ApplicationException">Thrown when the Column is not a primary key.</exception>
        internal static bool IsPrimaryKey([NotNull]this ICassandraFluentMigrator self, [NotNull]string table, [NotNull]string column)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            var session = self.GetCassandraSession();

            return session
                .Cluster
                .Metadata
                .GetTable(session.Keyspace, table.NormalizeString())
                .PartitionKeys
                .Any(x => x.Name.NormalizeString() == column.NormalizeString());
        }

        private static async Task<ICassandraFluentMigrator> ExecuteStatementAsync([NotNull] this ICassandraFluentMigrator self, [NotNull] string query, [NotNull]string errorMessage)
        {
            try
            {
                IStatement statement = new SimpleStatement(query);
                await self
                    .GetCassandraSession()
                    .ExecuteAsync(statement);
            }
            catch (Exception ex)
            {
                // Return the error if the message is other than the column exists.
                if (!ex.Message.NormalizeString().Contains(errorMessage.NormalizeString()))
                {
                    throw ex;
                }
            }

            return self;
        }
    }
}
