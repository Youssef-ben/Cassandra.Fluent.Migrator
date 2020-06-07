namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using System;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.Logging;

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

        public Version Version => new Version(1, 0, 3);

        public string Description => "Deleting the column {Language} from the users table.";

        public async Task ApplyMigrationAsync()
        {
            this.logger.LogInformation(this.Description);

            await this.cfm.DropColumnAsync("users", "language");
        }
    }
}
