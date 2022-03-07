namespace Cassandra.Fluent.Migrator
{
    using System.Diagnostics.CodeAnalysis;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    public static class CassandraFluentMigratorConfiguration
    {
        /// <summary>
        /// Adds the required services to the application service provider. <br/>
        /// Note: All the library classes are registered as Singleton.
        /// </summary>
        ///
        /// <param name="self">The service provider collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddCassandraFluentMigratorServices(this IServiceCollection self)
        {
            return self
                .AddSingleton<ICassandraFluentMigrator, CassandraFluentMigrator>()
                .AddSingleton<ICassandraMigrator, CassandraMigrator>();
        }

        /// <summary>
        /// Start the migration process.
        /// </summary>
        ///
        /// <param name="self">Application Builder.</param>
        /// <returns>The Application Builder.</returns>
        ///
        /// <exception cref="ObjectNotFoundException">Thrown when the Migrator is not registered in the Application Service provider.</exception>
        public static IApplicationBuilder UseCassandraMigration([NotNull]this IApplicationBuilder self)
        {
            Check.NotNull(self, $"The argument [Application Builder]");

            var migrator = self.ApplicationServices.GetService<ICassandraMigrator>();

            if (migrator is null)
            {
                throw new ObjectNotFoundException($"Couldn't find a registered Migrator. Please the [<IServiceCollection>.AddCassandraFluentMigratorServices()]");
            }

            migrator.Migrate();

            return self;
        }
    }
}
