namespace Cassandra.Fluent.Migrator.Helper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Utils.Constants;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Cassandra.Fluent.Migrator.Utils.Extensions;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    /// <summary>
    /// This partial Extensions class is used to group all the Cassandra {Table} Actions for the {Cassandra Fluent Migrator}.
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
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AddColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull] string column, [NotNull]Type type, bool shouldBeFrozen = false)
        {
            // Validate the parameters.
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotNull(type, $"The argument [{nameof(type)}]");

            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            return await self.ExecuteAddColumnAsync(table, column, type.GetCqlType(shouldBeFrozen));
        }

        /// <summary>
        /// Adds the specified column to the targeted table only if the column doesn't exists.
        ///
        /// <para>Note: The method automatically resolve the type of the column.</para>
        /// </summary>
        ///
        /// <typeparam name="Table">The Table where we should search for the column type.</typeparam>
        /// <param name="self">The instance of the Cassandra Fluent Migrator helper.</param>
        /// <param name="table">The table to which we want to add the new column.</param>
        /// <param name="column">The new column.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AddColumnAsync<Table>([NotNull]this ICassandraFluentMigrator self, [NotNull] string table, [NotNull] string column, bool shouldBeFrozen = false)
            where Table : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(table, $"The argument [{nameof(table)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            var typeName = self.GetColumnType<Table>(column, shouldBeFrozen);

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
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the Column is not a primary key or the target column name already exists.</exception>
        public static async Task<ICassandraFluentMigrator> RenamePrimaryColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string table, [NotNull]string old, [NotNull]string target)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [table]");
            Check.NotEmptyNotNull(old, $"The argument [old name]");
            Check.NotEmptyNotNull(target, $"The argument [new name]");

            table = table.NormalizeString();
            old = old.NormalizeString();
            target = target.NormalizeString();

            if (!self.DoesColumnExists(table, old))
            {
                return self;
            }

            if (self.DoesColumnExists(table, target))
            {
                throw new InvalidOperationException(AppErrorsMessages.TYPE_COLUMN_EXISTS.NormalizeString(target));
            }

            if (!self.IsPrimaryKey(table, old))
            {
                throw new InvalidOperationException(AppErrorsMessages.CAN_NOT_RENAME_NONE_PRIMARY_KEY.NormalizeString(old));
            }

            return await self.ExecuteRenameColumnQueryAsync(table, old, target);
        }

        /// <summary>
        /// Drops the specified column from the targeted table only if the column exists.
        /// </summary>
        ///
        /// <param name="self">The instance of the Cassandra Fluent Migrator helper.</param>
        /// <param name="table">The table from which we want to delete the column.</param>
        /// <param name="column">The column to be deleted.</param>
        /// <returns>The table Instance.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when one or all the specified arguments are invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> DropColumnAsync(this ICassandraFluentMigrator self, [NotNull]string table, [NotNull]string column)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator object]");
            Check.NotEmptyNotNull(table, $"The argument [{nameof(table)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            table = table.NormalizeString();
            column = column.NormalizeString();

            if (!self.DoesColumnExists(table, column))
            {
                return self;
            }

            return await self.ExecuteDropColumnQueryAsync(table, column);
        }

        private static async Task<ICassandraFluentMigrator> ExecuteAddColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string table, [NotNull] string column, [NotNull]string type)
        {
            table = table.NormalizeString();
            column = column.NormalizeString();
            type = type.NormalizeString();

            if (self.DoesColumnExists(table, column))
            {
                return self;
            }

            return await self.ExecuteCreateColumnQueryAsync(table, column, type);
        }
    }
}
