namespace Cassandra.Fluent.Migrator
{
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.DependencyInjection;

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
                .AddSingleton<ICassandraFluentMigrator, CassandraFluentMigrator>();
        }
    }
}
