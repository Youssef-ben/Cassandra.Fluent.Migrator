namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using System;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.Logging;

    public class IntialMigration : IMigrator
    {
        private readonly ICassandraFluentMigrator cfm;
        private readonly ILogger<IntialMigration> logger;

        public IntialMigration(ILogger<IntialMigration> logger, ICassandraFluentMigrator cfm)
        {
            this.cfm = cfm;
            this.logger = logger;
        }

        public string Name => "InitialMigration";

        public Version Version => new Version(1, 0, 0);

        public string Description => "First migration to initialize the Schema";

        public void ApplyMigration()
        {
            this.logger.LogInformation("Checking if the table exists...");
            var result = this.cfm.DoesTableExists("testingTable");

            this.logger.LogInformation("Returning result.");
        }
    }
}
