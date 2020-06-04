namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Utils.Constants;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class UdtExtensionsHelpers
    {
        /// <summary>
        /// Build an return the CQL statement to create a new type.
        /// The method automatically get the properties and their types based on the
        /// {TEntity} class.
        /// </summary>
        ///
        /// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
        /// <param name="self">The Cassandra Fluent Migrator.</param>
        /// <param name="name">The name of the udt.</param>
        /// <returns>The Cassandra CQL query.</returns>
        internal static string BuildCreateTypeQuery<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull] string name)
               where TEntity : class
        {
            Check.NotNull(self, $"The argument [cassandra fluent migrator]");
            Check.NotEmptyNotNull(name, $"The argument [{nameof(name)}]");

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

            return query.Append(");").ToString();
        }
    }
}
