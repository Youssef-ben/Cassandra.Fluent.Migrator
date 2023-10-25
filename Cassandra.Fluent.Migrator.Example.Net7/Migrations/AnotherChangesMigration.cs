namespace Cassandra.Fluent.Migrator.Example.Net7.Migrations;

using Core;
using Helper;

public class AnotherChangesMigration : IMigrator
{
    private readonly ICassandraFluentMigrator cfm;
    private readonly ILogger<AnotherChangesMigration> logger;

    public AnotherChangesMigration(ILogger<AnotherChangesMigration> logger, ICassandraFluentMigrator cfm)
    {
        this.cfm = cfm;
        this.logger = logger;
    }

    public string Name => this.GetType().Name;

    public Version Version => new(1, 0, 2);

    public string Description => "Renaming the column (id) to (guid) for users, add (Suite) for the Udt.";

    public async Task ApplyMigrationAsync()
    {
        logger.LogInformation(Description);
        await cfm.RenamePrimaryColumnAsync("users", "id", "guid");

        await cfm.AlterUdtAddColumnAsync("address", "suite", typeof(string));
    }
}