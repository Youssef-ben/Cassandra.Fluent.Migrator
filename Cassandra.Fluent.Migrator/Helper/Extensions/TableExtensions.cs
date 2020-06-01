﻿namespace Cassandra.Fluent.Migrator.Helper.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Utils.Constants;
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
        /// </summary>
        ///
        /// <param name="self">The instance of the Cassandra Fluent Migrator helper.</param>
        /// <param name="table">The table to which we want to add the new column.</param>
        /// <param name="column">The new column.</param>
        /// <param name="type">The column type.</param>
        /// <returns>The Cassandra Fluent Migrator helper.</returns>
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
        /// </summary>
        ///
        /// <typeparam name="Table">The Table where we should look for the column type.</typeparam>
        /// <param name="self">The instance of the Cassandra Fluent Migrator helper.</param>
        /// <param name="table">The table to which we want to add the new column.</param>
        /// <param name="column">The new column.</param>
        /// <returns>The Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AddColumnAsync<Table>([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull] string column)
            where Table : class
        {
            // Validate the parameters.
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(table, $"The argument [{nameof(table)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            // Get the type.
            var typeName = self.GetColumnType<Table>(column);

            return await self.ExecuteAddColumnAsync(table, column, typeName);
        }

        /// <summary>
        /// Rename the specified column in the targeted table only if the column exists.
        /// <para>IMPORTANT: In Cassandra only the Primary key can be renamed.</para>
        /// </summary>
        ///
        /// <param name="self">The instance of the Cassandra Fluent Migrator helper.</param>
        /// <param name="table">The table where we want to rename the column.</param>
        /// <param name="old">The column to be renamed.</param>
        /// <param name="target">The new column name.</param>
        /// <returns>The Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the Column is not a primary key.</exception>
        public static async Task<ICassandraFluentMigrator> RenamePrimaryColumnAsync(this ICassandraFluentMigrator self, string table, string old, string target)
        {
            // Validate the parameters.
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(old, $"The argument [old name]");
            Check.NotEmptyNotNull(target, $"The argument [new name]");

            // Normalize the values.
            table = table.NormalizeString();
            old = old.NormalizeString();
            target = target.NormalizeString();

            // Check if the table exists and the column exists.
            if (!self.DoesColumnExists(table, old))
            {
                return self;
            }

            if (!self.IsPrimaryKey(table, old))
            {
                throw new InvalidOperationException(AppErrorsMessages.CAN_NOT_RENAME_NONE_PRIMARY_KEY.NormalizeString(old));
            }

            // Create the query.
            return await self.ExecuteRenameColumnQueryAsync(table, old, target);
        }

        private static async Task<ICassandraFluentMigrator> ExecuteAddColumnAsync(this ICassandraFluentMigrator self, string table, string column, string type)
        {
            // Normalize the Strings.
            table = table.NormalizeString();
            column = column.NormalizeString();
            type = type.NormalizeString();

            // Check if the table exists and the column exists.
            if (self.DoesColumnExists(table, column))
            {
                return self;
            }

            // Execute the Query.
            return await self.ExecuteCreateColumnQueryAsync(table, column, type);
        }
    }
}