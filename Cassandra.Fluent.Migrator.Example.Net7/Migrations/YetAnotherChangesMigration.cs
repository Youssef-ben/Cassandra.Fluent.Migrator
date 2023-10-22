namespace Cassandra.Fluent.Migrator.Example.Net7.Migrations;

using Core;
using Helper;

public class YetAnotherChangesMigration : IMigrator
{
    private readonly ICassandraFluentMigrator cfm;
    private readonly ILogger<YetAnotherChangesMigration> logger;

    public YetAnotherChangesMigration(ILogger<YetAnotherChangesMigration> logger, ICassandraFluentMigrator cfm)
    {
        this.cfm = cfm;
        this.logger = logger;
    }

    public string Name => this.GetType().Name;

    public Version Version => new(1, 0, 3);

    public string Description => "Deleting the column {Language} from the users table.";

    public async Task ApplyMigrationAsync()
    {
        logger.LogInformation(Description);

        await cfm.DropColumnAsync("users", "language");
    }
}