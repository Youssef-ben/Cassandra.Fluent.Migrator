namespace Cassandra.Fluent.Migrator
{
    using System.Diagnostics.CodeAnalysis;
    using Core;
    using Helper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;
    using Utils.Exceptions;

    public static class CassandraFluentMigratorConfiguration
    {
        /// <summary>
        ///     Adds the required services to the application service provider. <br />
        /// </summary>
        /// <remarks>
        ///     Note: All the library classes are registered as Singleton.
        /// </remarks>
        /// <param name="self">The service provider collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddCassandraFluentMigratorServices(this IServiceCollection self)
        {
            return self
                    .AddSingleton<ICassandraFluentMigrator, CassandraFluentMigrator>()
                    .AddSingleton<ICassandraMigrator, CassandraMigrator>();
        }

        /// <summary>
        ///     Start the migration process.
        /// </summary>
        /// <remarks>
        ///     Note: The method makes sur that all the services are registered before starting the migrations.
        /// </remarks>
        /// <param name="self">Application Builder.</param>
        /// <returns>The Application Builder.</returns>
        /// <exception cref="ObjectNotFoundException">
        ///     Thrown when the Migrator is not registered in the Application Service
        ///     provider.
        /// </exception>
        public static IApplicationBuilder UseCassandraMigration([NotNull] this IApplicationBuilder self)
        {
            Check.NotNull(self, "The argument [Application Builder]");

            var migrator = self.ApplicationServices.GetService<ICassandraMigrator>();

            if (migrator is null)
            {
                var error = "Couldn't find a registered Migrator! ";
                error += "Please try adding the required service first (services.AddCassandraFluentMigratorServices())";
                throw new ObjectNotFoundException(error);
            }

            migrator.Migrate();

            return self;
        }
    }
}