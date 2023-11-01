namespace Cassandra.Fluent.Migrator.Tests.CassandraMigrator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configuration.Fixture;
using Configuration.Fixture.Docker;
using Core;
using Core.Models;
using Models.Domain;
using Xunit;
using Xunit.Priority;

/// <summary>
///     IMPORTANT NOTE: the use of {nameof(...)} in this test file is only
///     to make sure that we have a consistency and the tests don't break
///     BUT IN REAL WORLD application use MUST use a "string" for the migrations.
/// </summary>
[Collection(DockerComposeServiceFixtureCollection.COLLECTION_NAME)]
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CassandraMigratorTests : IClassFixture<CassandraMigratorFixture>
{
    private readonly CassandraMigratorFixture fixture;

    public CassandraMigratorTests(CassandraMigratorFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Priority(0)]
    public void GetRegisteredMigrations_Success()
    {
        ICollection<IMigrator> result = fixture.Migrator.GetRegisteredMigrations();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(new Version(1, 0, 0), result.FirstOrDefault()?.Version);
    }

    [Fact]
    [Priority(1)]
    public void GetAppliedMigration_Empty_Success()
    {
        ICollection<MigrationHistory> result = fixture.Migrator.GetAppliedMigrations();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    [Priority(2)]
    public void GetLatestMigration_Null_Success()
    {
        MigrationHistory result = fixture.Migrator.GetLatestMigration();

        Assert.Null(result);
    }

    [Fact]
    [Priority(3)]
    public void MigrateSchema_Success()
    {
        var result = fixture.Migrator.Migrate();

        Assert.Equal(1, result);

        Assert.True(fixture.MigratorHelper.DoesTableExists(nameof(Users)));
        Assert.True(fixture.MigratorHelper.DoesUdtExists(nameof(Address)));
    }

    [Fact]
    [Priority(4)]
    public void MigrateSchema_Nothing_Should_be_applied_Success()
    {
        var result = fixture.Migrator.Migrate();

        Assert.Equal(0, result);
    }

    [Fact]
    [Priority(5)]
    public void GetAppliedMigration_Success()
    {
        ICollection<MigrationHistory> result = fixture.Migrator.GetAppliedMigrations();

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        Assert.Equal("1.0.0", result.FirstOrDefault()?.Version);
    }

    [Fact]
    [Priority(5)]
    public void GetLatestMigration_Success()
    {
        MigrationHistory result = fixture.Migrator.GetLatestMigration();

        Assert.NotNull(result);
        Assert.Equal("1.0.0", result.Version);
    }

    [Fact]
    [Priority(100)]
    public async Task RemoveKeyspace_and_ShutdownTheSession()
    {
        ISession session = fixture.MigratorHelper.GetCassandraSession();
        Assert.NotNull(session);

        session.DeleteKeyspaceIfExists(session.Keyspace);
        await session.ShutdownAsync();
    }
}