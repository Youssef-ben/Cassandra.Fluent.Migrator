namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using Core;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationIoc
    {
        public static IServiceCollection AddCassandraMigrations(this IServiceCollection self)
        {
            return self
                    .AddTransient<IMigrator, InitialMigration>()
                    .AddTransient<IMigrator, AnotherChangesMigration>()
                    .AddTransient<IMigrator, AddActiveColumnToUsersMigration>()
                    .AddTransient<IMigrator, YetAnotherChangesMigration>()
                    .AddTransient<IMigrator, AddingNewTypeMigration>();
        }
    }
}