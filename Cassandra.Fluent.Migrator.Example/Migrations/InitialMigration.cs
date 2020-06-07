namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using System;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Example.Models.Domain;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.Logging;

    public class InitialMigration : IMigrator
    {
        private readonly ICassandraFluentMigrator cfm;
        private readonly ILogger<InitialMigration> logger;

        public InitialMigration(ILogger<InitialMigration> logger, ICassandraFluentMigrator cfm)
        {
            this.cfm = cfm;
            this.logger = logger;
        }

        public string Name => this.GetType().Name;

        public Version Version => new Version(1, 0, 0);

        public string Description => "First migration to initialize the Schema";

        public async Task ApplyMigrationAsync()
        {
            this.logger.LogDebug($"Creating the Address User-Defined type...");
            await this.cfm.CreateUserDefinedTypeAsync<Address>();

            // Shold not be here, for the example purposes.
            this.cfm
                .GetCassandraSession()
                 .UserDefinedTypes.Define(
                    UdtMap.For<Address>()
                       .Map(a => a.Number, "Number".ToLower())
                       .Map(a => a.Street, "Street".ToLower())
                       .Map(a => a.City, "City".ToLower())
                       .Map(a => a.Contry, "Contry".ToLower())
                       .Map(a => a.Province, "Province".ToLower())
                       .Map(a => a.PostalCode, "PostalCode".ToLower()));

            this.logger.LogDebug($"Creating the User table...");
            await this.cfm.GetTable<Users>().CreateIfNotExistsAsync();
        }
    }
}
