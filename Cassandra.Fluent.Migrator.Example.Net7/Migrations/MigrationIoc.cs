namespace Cassandra.Fluent.Migrator.Example.Net7.Migrations;

using Core;

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