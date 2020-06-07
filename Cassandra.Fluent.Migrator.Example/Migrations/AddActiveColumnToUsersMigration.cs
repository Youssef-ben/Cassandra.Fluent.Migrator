namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using System;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.Logging;

    public class AddActiveColumnToUsersMigration : IMigrator
    {
        private readonly ICassandraFluentMigrator cfm;
        private readonly ILogger<AddActiveColumnToUsersMigration> logger;

        public AddActiveColumnToUsersMigration(ILogger<AddActiveColumnToUsersMigration> logger, ICassandraFluentMigrator cfm)
        {
            this.cfm = cfm;
            this.logger = logger;
        }

        public string Name => this.GetType().Name;

        public Version Version => new Version(1, 0, 1);

        public string Description => "Adding a new column {active} to the Users table.";

        public async Task ApplyMigrationAsync()
        {
            this.logger.LogDebug("Adding a new column {active} to the user...");
            await this.cfm.AddColumnAsync("users", "active", typeof(bool));
        }
    }
}
