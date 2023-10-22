namespace Cassandra.Fluent.Migrator.Tests.Migrations;

using System;
using System.Threading.Tasks;
using Core;
using Helper;
using Microsoft.Extensions.Logging;
using Models.Domain;

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

    public Version Version => new(1, 0, 0);

    public string Description => "First migration to initialize the Schema";

    public async Task ApplyMigrationAsync()
    {
        logger.LogDebug("Creating the Address User-Defined type...");
        await cfm.CreateUserDefinedTypeAsync<Address>();

        // Should not be here, for the example purposes.
        await cfm
                .GetCassandraSession()
                .UserDefinedTypes.DefineAsync(
                        UdtMap.For<Address>()
                                .Map(a => a.Number, "Number".ToLower())
                                .Map(a => a.Street, "Street".ToLower())
                                .Map(a => a.City, "City".ToLower())
                                .Map(a => a.Country, "Country".ToLower())
                                .Map(a => a.Province, "Province".ToLower())
                                .Map(a => a.PostalCode, "PostalCode".ToLower()));

        logger.LogDebug("Creating the User table...");
        await cfm.GetTable<Users>().CreateIfNotExistsAsync();
    }
}