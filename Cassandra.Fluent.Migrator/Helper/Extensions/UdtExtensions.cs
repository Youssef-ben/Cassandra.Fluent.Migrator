namespace Cassandra.Fluent.Migrator.Helper.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
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
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the udt (Optional).</param>
        /// <returns>The Cassandra CQL query.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
        public static async Task<ICassandraFluentMigrator> CreateUserDefinedTypeAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, string name = default)
            where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");

            name = string.IsNullOrWhiteSpace(name) ? typeof(TEntity).Name.NormalizeString() : name.NormalizeString();

            // In case the UDT exists, stop cancel the method.
            if (self.DoesUdtExists(name))
            {
                return self;
            }

            // Build Query.
            var query = self.BuildCreateTypeQuery<TEntity>(name);

            // Execute Query.
            IStatement statement = new SimpleStatement(query);
            await self
                .GetCassandraSession()
                .ExecuteAsync(statement);

            return self;
        }
    }
}
