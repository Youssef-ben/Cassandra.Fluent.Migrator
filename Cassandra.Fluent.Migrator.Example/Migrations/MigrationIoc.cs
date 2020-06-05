namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using Cassandra.Fluent.Migrator.Core;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationIoc
    {
        public static IServiceCollection AddMigrations(this IServiceCollection self)
        {
            return self.
                AddTransient<IMigrator, IntialMigration>();
        }
    }
}
