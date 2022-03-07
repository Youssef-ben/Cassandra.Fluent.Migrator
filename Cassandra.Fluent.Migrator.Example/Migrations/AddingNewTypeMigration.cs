namespace Cassandra.Fluent.Migrator.Example.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Example.Models.Domain;
    using Cassandra.Fluent.Migrator.Helper;

    public class AddingNewTypeMigration : IMigrator
    {
        private readonly ICassandraFluentMigrator cfmHelper;

        public AddingNewTypeMigration(ICassandraFluentMigrator cfmHelper)
        {
            this.cfmHelper = cfmHelper;
        }

        public string Name => this.GetType().Name;

        public Version Version => new Version(1, 0, 4);

        public string Description => "Adding a new type to database.";

        public async Task ApplyMigrationAsync()
        {
            await this.cfmHelper.CreateUserDefinedTypeAsync<NewCassandraType>();

            // Should not be here, for the example purposes.
            this.cfmHelper
                .GetCassandraSession()
                 .UserDefinedTypes.Define(
                    UdtMap.For<NewCassandraType>()
                       .Map(a => a.Id, "Id".ToLower())
                       .Map(a => a.Name, "Name".ToLower()));

            await this.cfmHelper.AddColumnAsync("users", "newColumn", typeof(ICollection<NewCassandraType>));
        }
    }
}
