namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using System;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.Logging;

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

        public Version Version => new Version(1, 0, 2);

        public string Description => "Renaming the column {id} to {guid} for users, add {Suite} for the Udt.";

        public async Task ApplyMigrationAsync()
        {
            this.logger.LogInformation(this.Description);
            await this.cfm.RenamePrimaryColumnAsync("users", "id", "guid");

            await this.cfm.AlterUdtAddColumnAsync("address", "suite", typeof(string));
        }
    }
}
