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
    /// This partial Extensions class is used to group all the Cassandra User-Defined types Actions.
    /// </summary>
    public static partial class CfmExtensions
    {
        /// <summary>
        /// Create the new User-Defined type by building and generating a query
        /// based on the generic class fields and types.
        /// If the UDT already exists the method skips the creation.
        ///
        /// <para>Note: If the udt name is [Null || Empty] the method will take the generic type {TEntity} name.</para>
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the udt (Optional).</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> CreateUserDefinedTypeAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, string name = default, bool shouldBeFrozen = false)
            where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");

            name = string.IsNullOrWhiteSpace(name) ? typeof(TEntity).Name.NormalizeString() : name.NormalizeString();

            // Check if the UDT doesn't exists, otherwise stop the method.
            if (self.DoesUdtExists(name))
            {
                return self;
            }

            // Execute Query.
            return await self.ExecuteBuildUdtAsync<TEntity>(name, shouldBeFrozen);
        }

        /// <summary>
        /// Delete the User-Defined type if exists.
        /// If the UDT doesn't exists the method skips the creation.
        ///
        /// <para>Note: If the udt name is [Null || Empty] the method will take the generic type {TEntity} name.</para>
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the udt (Optional).</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> DeleteUserDefinedTypeAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull]string name = default)
            where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");

            name = string.IsNullOrWhiteSpace(name) ? typeof(TEntity).Name.NormalizeString() : name.NormalizeString();

            // In case the UDT exists, stop cancel the method.
            if (!self.DoesUdtExists(name))
            {
                return self;
            }

            // Execute Query.
            return await self.ExecuteDeleteUdtAsync<TEntity>(name);
        }

        /// <summary>
        /// Alter the specified Uder-Defined type by adding a new column only if it doesn't exists.
        /// If the UDT exists the method skips the creation.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be added.</param>
        /// <param name="type">The type of the new column.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the udt doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AlterUdtAddColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, string column, Type type, bool shouldBeFrozen = false)
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotNull(type, $"The argument [{nameof(type)}]");

            Check.NotEmptyNotNull(udt, $"The argument [User-Defined type (udt)]");
            Check.NotNull(column, $"The argument [{nameof(column)}]");

            // Check if the UDT doesn't exists. Otherwise do nothing.
            if (self.DoesUdtColumnExists(udt, column))
            {
                return self;
            }

            return await self.ExecuteAlterUdtAddColumnQuery(udt, column, type.GetCqlType(shouldBeFrozen));
        }

        /// <summary>
        /// Alter the specified Uder-Defined type by adding a new column only if it doesn't exists.
        /// If the UDT exists the method skips the creation.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be added.</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the udt doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AlterUdtAddColumnAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, string column)
            where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");

            Check.NotEmptyNotNull(udt, $"The argument [User-Defined type (udt)]");
            Check.NotNull(column, $"The argument [{nameof(column)}]");

            // Check if the UDT doesn't exists. Otherwise do nothing.
            if (self.DoesUdtColumnExists(udt, column))
            {
                return self;
            }

            // Get the type.
            var typeName = self.GetColumnType<TEntity>(column);

            return await self.ExecuteAlterUdtAddColumnQuery(udt, column, typeName);
        }

        /// <summary>
        /// Alter the specified User-Defined type by renaming the column name by the target name.
        /// In case the target name exists the method throws an exception.
        /// </summary>
        ///
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="udt">The name of the User-Defined type.</param>
        /// <param name="column">The name of the column to be renamed.</param>
        /// <param name="target">The new column name.</param>
        /// <returns>The Cassandra Fluent Migrator helper.</returns>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the target column name exists.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the udt doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> AlterUdtRenameColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, [NotNull]string column, [NotNull]string target)
        {
            Check.NotNull(self, "The argument [Cassandra Fluent Migrator]");
            Check.NotEmptyNotNull(udt, $"The argument [User-Defined type]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");
            Check.NotEmptyNotNull(target, $"The argument [{nameof(target)}]");

            // If the column doesn't exits skip the rename action.
            if (!self.DoesUdtColumnExists(udt, column))
            {
                return self;
            }

            // If the target name exists, throw an exception.
            if (self.DoesUdtColumnExists(udt, target))
            {
                throw new InvalidOperationException(AppErrorsMessages.TYPE_UDT_COLUMN_EXISTS.NormalizeString(target));
            }

            return await self.ExecuteAlterUdtRenameColumnQuery(udt, column, target);
        }
    }
}
