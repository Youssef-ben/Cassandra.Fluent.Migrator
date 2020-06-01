namespace Cassandra.Fluent.Migrator.Helper.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Cassandra.Fluent.Migrator.Utils.Extensions;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    /// <summary>
    /// This partial Extensions class is used to group all the Cassandra Table Actions.
    /// </summary>
    public static partial class CfmExtensions
    {
        /// <summary>
        /// Adds the specified column to the targeted table only if the column doesn't exists.
        ///
        /// <para>Note: the method return the instance of the calling Table to allow writing a fluent code.</para>
        /// </summary>
        ///
        /// <param name="self">The instance of the Cassandra Fluent Migratr helper.</param>
        /// <param name="table">The targeted table.</param>
        /// <param name="column">The new columns.</param>
        /// <param name="type">The new column type.</param>
        /// <returns>The instance of the Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AddColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull] string column, [NotNull]Type type)
        {
            // Validate the parameters.
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotNull(type, $"The argument [{nameof(type)}]");

            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            return await self.ExecuteAddColumnAsync(table, column, type.GetCqlType());
        }

        /// <summary>
        /// Adds the specified column to the targeted table only if the column doesn't exists.
        /// The method automatically resolve the type of the column.
        ///
        /// <para>Note: the method return the instance of the calling Table to allow writing a fluent code.</para>
        /// </summary>
        ///
        /// <typeparam name="Table">The Table where we should look for the column type.</typeparam>
        /// <param name="self">The instance of the Cassandra Fluent Migratr helper.</param>
        /// <param name="column">The new columns.</param>
        /// <returns>The instance of the Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AddColumnAsync<Table>([NotNull]this ICassandraFluentMigrator self, [NotNull] string column)
            where Table : class
        {
            // Validate the parameters.
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            // Get the type.
            var typeName = self.GetColumnType<Table>(column);
            var table = typeof(Table).Name;

            return await self.ExecuteAddColumnAsync(table, column, typeName);
        }

        private static async Task<ICassandraFluentMigrator> ExecuteAddColumnAsync(this ICassandraFluentMigrator self, string table, string column, string type)
        {
            // Normalize the Strings.
            table = table.NormalizeString();
            column = column.NormalizeString();
            type = type.NormalizeString();

            // Check if the table exists and the column doesn't exists.
            if (self.DoesColumnExists(table, column))
            {
                return self;
            }

            // Create the Query.
            await self
                .ExecuteCreateColumnQueryAsync(table, column, type);

            return self;
        }
    }
}
