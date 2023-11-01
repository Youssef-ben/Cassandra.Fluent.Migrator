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
public class CfmTableExtensionsTests : IClassFixture<CassandraMigratorFixture>
{
    private readonly CassandraMigratorFixture fixture;

    public CfmTableExtensionsTests(CassandraMigratorFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Priority(0)]
    public async Task Initialize()
    {
        var result = fixture.MigratorHelper.DoesTableExists(nameof(CfmHelperObject));
        Assert.False(result);

        /*
         * Ensure that the Table we want to create exists.
         * This was created based on the object {CfmHelperObject} that normally contains 3 field,
         * but in this method we created only 2 fields for testing purposes.
         */
        var query = $"CREATE TABLE IF NOT EXISTS {nameof(CfmHelperObject)}(id int, values text, PRIMARY KEY (id))";
        IStatement statement = new SimpleStatement(query);
        await fixture.GetSession().ExecuteAsync(statement);

        result = fixture.MigratorHelper.DoesTableExists(nameof(CfmHelperObject));
        Assert.True(result);
    }

    [Fact]
    [Priority(1)]
    public async Task AddColumn_TypeSpecified_Success()
    {
        var column = "AddedColumnFromTest";

        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), column);
        Assert.False(result);

        await fixture.MigratorHelper.AddColumnAsync(nameof(CfmHelperObject), column, typeof(string));

        result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), column);
        Assert.True(result);
    }

    [Fact]
    [Priority(1)]
    public async Task AddColumn_TypeNotSpecified_Success()
    {
        var column = "AddedColumnFromTestWithoutType";

        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), column);
        Assert.False(result);

        await fixture.MigratorHelper.AddColumnAsync<CfmHelperObject>(nameof(CfmHelperObject), column);

        result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), column);
        Assert.True(result);
    }

    [Fact]
    [Priority(2)]
    public async Task RenamePrimaryKey_Success()
    {
        var old = "id";
        var target = "renamedId";

        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), old);
        Assert.True(result);

        result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), target);
        Assert.False(result);

        await fixture.MigratorHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), old, target);

        result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), old);
        Assert.False(result);

        result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), target);
        Assert.True(result);
    }

    [Fact]
    [Priority(3)]
    public async Task RenamePrimaryKey_DoesntExists_SKIPPED()
    {
        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), "id");
        Assert.False(result);

        await fixture.MigratorHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), "id", "renamedId");
    }

    [Fact]
    [Priority(4)]
    public async Task RenamePrimaryKey_ColumnNotPrimary_Failed()
    {
        try
        {
            await fixture.MigratorHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), "Values", "NotImportant");
        }
        catch (InvalidOperationException ex)
        {
            Assert.Contains("the [values] is not a primary key. you can only rename primary keys!".ToLower(),
                    ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(4)]
    public async Task RenamePrimaryKey_TargetNameAlreadyExists_Failed()
    {
        try
        {
            await fixture.MigratorHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), "Values", "renamedid");
        }
        catch (InvalidOperationException ex)
        {
            Assert.Contains("a field of the same name already exists!".ToLower(), ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(5)]
    public async Task DeleteColumn_Success()
    {
        var column = "AddedColumnFromTestWithoutType";

        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), column);
        Assert.True(result);

        await fixture.MigratorHelper.DropColumnAsync(nameof(CfmHelperObject), column);

        result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), column);
        Assert.False(result);
    }

    [Fact]
    [Priority(5)]
    public async Task DeleteColumn_Failed()
    {
        try
        {
            await fixture.MigratorHelper.DropColumnAsync("TableDoesntExists", "AddedColumnFromTestWithoutType");
        }
        catch (ObjectNotFoundException ex)
        {
            var notFound = "the table [tabledoesntexists], was not found in the specified cassandra".ToLower();
            Assert.Contains(notFound, ex.Message.ToLower());
        }
    }
}