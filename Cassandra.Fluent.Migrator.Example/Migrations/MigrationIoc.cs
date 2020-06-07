namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using Cassandra.Fluent.Migrator.Core;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationIoc
    {
        public static IServiceCollection AddCassandraMigrations(this IServiceCollection self)
        {
            return self
                .AddTransient<IMigrator, InitialMigration>()
                .AddTransient<IMigrator, AddActiveColumnToUsersMigration>()
                .AddTransient<IMigrator, AnotherChangesMigration>()
                .AddTransient<IMigrator, YetAnotherChangesMigration>()
                .AddTransient<IMigrator, AddingNewTypeMigration>();
        }
    }
}
