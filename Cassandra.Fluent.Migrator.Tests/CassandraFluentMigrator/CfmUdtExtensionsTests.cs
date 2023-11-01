namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator;

using System;
using System.Threading.Tasks;
using Configuration.Fixture;
using Configuration.Fixture.Docker;
using Helper;
using Models;
using Utils.Exceptions;
using Xunit;
using Xunit.Priority;

/// <summary>
///     IMPORTANT NOTE: the use of {nameof(...)} in this test file is only
///     to make sure that we have a consistency and the tests don't break
///     BUT IN REAL WORLD application use MUST use a "string" for the migrations.
/// </summary>
[Collection(DockerComposeServiceFixtureCollection.COLLECTION_NAME)]
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CfmUdtExtensionsTests : IClassFixture<CassandraMigratorFixture>
{
    private const string CANT_RENAME = "cannot add new field [{0}] to type!";
    private const string UDT_NOT_FOUND = "the User-Defined type [{0}], was not found in the specified cassandra";

    private readonly CassandraMigratorFixture fixture;

    public CfmUdtExtensionsTests(CassandraMigratorFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Priority(0)]
    public async Task CreateUserDefinedType_Success()
    {
        var udtName = nameof(CfmHelperObject);

        var result = fixture.MigratorHelper.DoesUdtExists(udtName);
        Assert.False(result);

        await fixture.MigratorHelper.CreateUserDefinedTypeAsync<CfmHelperObject>();

        result = fixture.MigratorHelper.DoesUdtExists(udtName);
        Assert.True(result);
    }

    [Fact]
    [Priority(1)]
    public async Task DeleteTheUserDefinedType_Success()
    {
        var udtName = nameof(CfmHelperObject);

        var result = fixture.MigratorHelper.DoesUdtExists(udtName);
        Assert.True(result);

        await fixture.MigratorHelper.DeleteUserDefinedTypeAsync<CfmHelperObject>();

        result = fixture.MigratorHelper.DoesUdtExists(udtName);
        Assert.False(result);
    }

    [Fact]
    [Priority(2)]
    public async Task Initialize_ForTheRestOfTests()
    {
        var result = fixture.MigratorHelper.DoesUdtExists(nameof(CfmHelperObject));
        Assert.False(result);

        /*
         * Ensure that the UDT we want to create exists.
         * This was created based on the object {CfmHelperObject} that normally contains 3 field,
         * but in this method we created only 2 fields for testing purposes.
         */
        IStatement statement =
                new SimpleStatement($"CREATE TYPE IF NOT EXISTS {nameof(CfmHelperObject)} (id int, values text)");
        await fixture.GetSession().ExecuteAsync(statement);

        result = fixture.MigratorHelper.DoesUdtExists(nameof(CfmHelperObject));
        Assert.True(result);
    }

    [Fact]
    [Priority(3)]
    public async Task AddColumn_Success()
    {
        var column = "AddedColumnFromTest";

        var result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
        Assert.False(result);

        await fixture.MigratorHelper.AlterUdtAddColumnAsync(nameof(CfmHelperObject), column, typeof(bool));

        result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
        Assert.True(result);
    }

    [Fact]
    [Priority(3)]
    public async Task AddColumn_WithoutType_Success()
    {
        var column = nameof(CfmHelperObject.AddedColumnFromTestWithoutType);

        var result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
        Assert.False(result);

        await fixture.MigratorHelper.AlterUdtAddColumnAsync<CfmHelperObject>(nameof(CfmHelperObject), column);

        result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
        Assert.True(result);
    }

    [Fact]
    [Priority(3)]
    public async Task AddColumn_UdtNotFound_Failed()
    {
        try
        {
            await fixture.MigratorHelper.AlterUdtAddColumnAsync("DoesntExists", "Values", typeof(string));
        }
        catch (ObjectNotFoundException ex)
        {
            var expected = string.Format(UDT_NOT_FOUND, "DoesntExists").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(4)]
    public async Task RenameColumn_Success()
    {
        var column = "values";
        var target = "valuesDetails";

        var result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
        Assert.True(result);

        result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), target);
        Assert.False(result);

        // Execute
        await fixture.MigratorHelper.AlterUdtRenameColumnAsync(nameof(CfmHelperObject), column, target);

        result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
        Assert.False(result);

        result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), target);
        Assert.True(result);
    }

    [Fact]
    [Priority(4)]
    public async Task RenameColumn_TargetColumnExists_Failed()
    {
        try
        {
            await fixture.MigratorHelper
                    .AlterUdtRenameColumnAsync(nameof(CfmHelperObject), "AddedColumnFromTest", "id");
        }
        catch (InvalidOperationException ex)
        {
            var expected = string.Format(CANT_RENAME, "id").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(4)]
    public async Task RenameColumn_UdtNotFound_Failed()
    {
        try
        {
            await fixture.MigratorHelper.AlterUdtRenameColumnAsync("DoesntExists", "Values", "valuesDetails");
        }
        catch (ObjectNotFoundException ex)
        {
            var expected = string.Format(UDT_NOT_FOUND, "DoesntExists").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }
}