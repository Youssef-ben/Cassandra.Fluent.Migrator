namespace Cassandra.Fluent.Migrator.Example.Net7.Migrations;

using Core;
using Helper;

public class AddActiveColumnToUsersMigration : IMigrator
{
    private readonly ICassandraFluentMigrator cfm;
    private readonly ILogger<AddActiveColumnToUsersMigration> logger;

    public AddActiveColumnToUsersMigration(ILogger<AddActiveColumnToUsersMigration> logger,
            ICassandraFluentMigrator cfm)
    {
        this.cfm = cfm;
        this.logger = logger;
    }

    public string Name => this.GetType().Name;

    public Version Version => new(1, 0, 1);

    public string Description => "Adding a new column {active} to the Users table.";

    public async Task ApplyMigrationAsync()
    {
        logger.LogDebug("Adding a new column (active) to the user...");
        await cfm.AddColumnAsync("users", "active", typeof(bool));
    }
}