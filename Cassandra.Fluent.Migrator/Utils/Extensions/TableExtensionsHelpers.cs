namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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
        internal static async Task ExecuteCreateColumnQueryAsync([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull]string column, [NotNull] string type)
        {
            Check.NotNull(self, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            table = table.NormalizeString();
            column = column.NormalizeString();
            type = type.NormalizeString();

            var query = TableCqlStatements.TABLE_ADD_COLUMN_STATEMENT.NormalizeString(table, column, type);

            await self.ExecuteStatementAsync(query, AppErrorsMessages.COLUMN_EXISTS.NormalizeString(column));
        }

        private static async Task ExecuteStatementAsync([NotNull] this ICassandraFluentMigrator self, [NotNull] string query, [NotNull]string errorMessage)
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
        }
    }
}
