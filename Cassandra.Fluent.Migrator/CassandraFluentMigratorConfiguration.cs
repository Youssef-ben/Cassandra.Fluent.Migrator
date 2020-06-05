namespace Cassandra.Fluent.Migrator
{
    using System.Diagnostics.CodeAnalysis;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    public static class CassandraFluentMigratorConfiguration
    {
        /// <summary>
        /// Adds the required services to the application service provider. <br/>
        /// Note: All the library classes are registred as Singleton.
        /// </summary>
        /// <param name="self">The service provider collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddCassandraFluentMigratorServices(this IServiceCollection self)
        {
            return self
                .AddTransient<ICassandraFluentMigrator, CassandraFluentMigrator>()
                .AddTransient<ICassandraMigrator, CassandraMigrator>();
        }

        public static IApplicationBuilder UseCassandraMigration([NotNull]this IApplicationBuilder self)
        {
            Check.NotNull(self, $"The argument [Application Builder]");

            // Get the Migrator from the ServiceProvider
            var migrator = self.ApplicationServices.GetService<ICassandraMigrator>();

            // Start the migration.
            migrator.Migrate();

            return self;
        }
    }
}
