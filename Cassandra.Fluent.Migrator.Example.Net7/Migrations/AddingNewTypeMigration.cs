namespace Cassandra.Fluent.Migrator.Example.Net7.Migrations;

using Core;
using Helper;
using Models.Domain;

public class AddingNewTypeMigration : IMigrator
{
    private readonly ICassandraFluentMigrator cfmHelper;

    public AddingNewTypeMigration(ICassandraFluentMigrator cfmHelper)
    {
        this.cfmHelper = cfmHelper;
    }

    public string Name => this.GetType().Name;

    public Version Version => new(1, 0, 4);

    public string Description => "Adding a new type to database.";

    public async Task ApplyMigrationAsync()
    {
        await cfmHelper.CreateUserDefinedTypeAsync<NewCassandraType>();

        // Should not be here, for the example purposes.
        await cfmHelper
                .GetCassandraSession()
                .UserDefinedTypes.DefineAsync(
                        UdtMap.For<NewCassandraType>()
                                .Map(a => a.Id, "Id".ToLower())
                                .Map(a => a.Name, "Name".ToLower()));

        await cfmHelper.AddColumnAsync("users", "newColumn", typeof(ICollection<NewCassandraType>));
    }
}