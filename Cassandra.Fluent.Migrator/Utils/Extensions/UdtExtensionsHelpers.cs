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
        /// Build and execute the create User-Defined type query statement.
        /// The method automatically get the properties and their types based on the
        /// {TEntity} class.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The class where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the User-Defined type.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteBuildUdtAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull] string name, bool shouldBeFrozen)
               where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(name, $"The argument [{nameof(name)}]");

            var count = 0;

            var properties = typeof(TEntity).GetProperties();

            var query = new StringBuilder(UdtCqlStatements.TYPE_CREATE_STATEMENT.NormalizeString(name));

            foreach (var property in properties)
            {
                var propName = property.Name.NormalizeString();
                var propType = property.PropertyType.GetCqlType(shouldBeFrozen);

                query.Append($"{propName} {propType}");

                if (count < properties.Length - 1)
                {
                    query.Append(", ");
                    count++;
                }
            }

            var statement = query
                .Append(");")
                .ToString()
                .NormalizeString();

            return await self.ExecuteUdtStatementAsync(statement);
        }

        /// <summary>
        /// Build and execute the delete User-Defined type query statement.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The class where the that represent the User-Defined type.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the User-Defined type (Optional: will be taken from the {TEntity} if not specified).</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
        internal static async Task<ICassandraFluentMigrator> ExecuteDeleteUdtAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull] string name)
              where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(name, $"The argument [{nameof(name)}]");

            var query = UdtCqlStatements.TYPE_DROP_STATEMENT.NormalizeString(name);

            return await self.ExecuteUdtStatementAsync(query);
        }

        /// <summary>
        /// Build and execute the Add Column query statement for the User-Defined types.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be added.</param>
        /// <param name="type">The type of the column.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
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
        /// Build and execute the Rename Column query statement for the User-Defined types.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be renamed.</param>
        /// <param name="target">The new column name.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are empty or null.</exception>
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
