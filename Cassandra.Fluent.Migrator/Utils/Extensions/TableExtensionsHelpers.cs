namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Utils.Constants;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    public static class TableExtensionsHelpers
    {
        /// <summary>
        /// Build and execute the Add columns query statement.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="column">The column that we want to add.</param>
        /// <param name="type">The type of the columns.</param>
        /// <returns>Nothing.</returns>
        internal static async Task<ICassandraFluentMigrator> ExecuteCreateColumnQueryAsync([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull]string column, [NotNull] string type)
        {
            Check.NotNull(self, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(type)}]");

            table = table.NormalizeString();
            column = column.NormalizeString();
            type = type.NormalizeString();

            var query = TableCqlStatements.TABLE_ADD_COLUMN_STATEMENT.NormalizeString(table, column, type);

            return await self.ExecuteStatementAsync(query, AppErrorsMessages.COLUMN_EXISTS.NormalizeString(column));
        }

        /// <summary>
        /// Build the Reanme Column Query statement and execute it.
        /// </summary>
        ///
        /// <param name="self">Cassandra Table.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="oldColumn">Old Column to rename.</param>
        /// <param name="targetName">New Column name.</param>
        /// <returns>Nothing.</returns>
        internal static async Task<ICassandraFluentMigrator> ExecuteRenameColumnQueryAsync([NotNull]this ICassandraFluentMigrator self, string table, string oldColumn, string targetName)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(oldColumn, $"The argument [old name]");
            Check.NotEmptyNotNull(targetName, $"The argument [new name]");

            table = table.NormalizeString();
            oldColumn = oldColumn.NormalizeString();
            targetName = targetName.NormalizeString();

            var query = TableCqlStatements.TABLE_RENAME_COLUMN_STATEMENT.NormalizeString(table, oldColumn, targetName);

            return await self.ExecuteStatementAsync(query, AppErrorsMessages.COLUMN_EXISTS_FOR_RENAME.NormalizeString(oldColumn, targetName, self.GetCassandraSession().Keyspace));
        }

        /// <summary>
        /// Check if the specified column is a [PrimaryKey].
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="table">The Cassandra table name.</param>
        /// <param name="column">The Column to check.</param>
        /// <returns>True if Primary, False Otherwise.</returns>
        /// <exception cref="ApplicationException">Thrown when the Column is not a primary key.</exception>
        internal static bool IsPrimaryKey([NotNull]this ICassandraFluentMigrator self, [NotNull]string table, [NotNull] string column)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            var session = self.GetCassandraSession();

            return session
                .Cluster
                .Metadata
                .GetTable(session.Keyspace, table)
                .PartitionKeys
                .Any(x => x.Name == column);
        }

        private static async Task<ICassandraFluentMigrator> ExecuteStatementAsync([NotNull] this ICassandraFluentMigrator self, [NotNull] string query, [NotNull]string errorMessage)
        {
            Check.NotNull(self, "The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(query, "The argument [query]");
            Check.NotEmptyNotNull(errorMessage, "The argument [pre-build error message]");

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
