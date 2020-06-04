namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Utils.Constants;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class UdtExtensionsHelpers
    {
        /// <summary>
        /// Build and execute the CQL statement to create a new type.
        /// The method automatically get the properties and their types based on the
        /// {TEntity} class.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the udt.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteBuildUdtAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull] string name)
               where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(name, $"The argument [{nameof(name)}]");

            name = name.NormalizeString();

            var properties = typeof(TEntity).GetProperties();

            var count = 0;

            var query = new StringBuilder(UdtCqlStatements.TYPE_CREATE_STATEMENT.NormalizeString(name));

            foreach (var property in properties)
            {
                var propName = property.Name.NormalizeString();
                var propType = property.PropertyType.GetCqlType().NormalizeString();

                query.Append($"{propName} {propType}");

                if (count < properties.Length - 1)
                {
                    query.Append(", ");
                    count++;
                }
            }

            var statement = query.Append(");").ToString();

            return await self.ExecuteUdtStatementAsync(statement);
        }

        /// <summary>
        /// Build and excecute the CQL statement to delet th specified type.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the udt.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteDeleteUdtAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull] string name)
              where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(name, $"The argument [{nameof(name)}]");

            // Build Query.
            var query = UdtCqlStatements.TYPE_DROP_STATEMENT.NormalizeString(name);

            return await self.ExecuteUdtStatementAsync(query);
        }

        /// <summary>
        /// Build the Add Column Query statement for the User-Defined types and execute it.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be added.</param>
        /// <param name="type">The type of the new column.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteAlterUdtAddColumnQuery([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, [NotNull]string column, [NotNull]string type)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");

            Check.NotEmptyNotNull(udt, $"The argument [User-Defined type (udt)]");
            Check.NotEmptyNotNull(type, $"The argument [{nameof(type)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            var query = UdtCqlStatements.TYPE_ADD_COLUMN_STATEMENT.NormalizeString(udt, column, type);
            return await self.ExecuteUdtStatementAsync(query, AppErrorsMessages.TYPE_UDT_COLUMN_EXISTS.NormalizeString(column));
        }

        /// <summary>
        /// Build the Rename Column Query statement for the User-Defined types and execute it.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be renamed.</param>
        /// <param name="target">The new column name.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteAlterUdtRenameColumnQuery([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, [NotNull]string column, [NotNull]string target)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");

            Check.NotEmptyNotNull(udt, $"The argument [User-Defined type (udt)]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");
            Check.NotEmptyNotNull(target, $"The argument [{nameof(target)}]");

            var query = UdtCqlStatements.TYPE_RENAME_COLUMN_STATEMENT.NormalizeString(udt, column, target);

            return await self.ExecuteUdtStatementAsync(query, AppErrorsMessages.TYPE_COLUMN_NOT_FOUND.NormalizeString(column));
        }

        private static async Task<ICassandraFluentMigrator> ExecuteUdtStatementAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string query, [NotNull]string errorMessage = "throw error")
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(query, $"The argument [{nameof(query)}]");
            Check.NotEmptyNotNull(errorMessage, $"The argument [Expected error message]");

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
