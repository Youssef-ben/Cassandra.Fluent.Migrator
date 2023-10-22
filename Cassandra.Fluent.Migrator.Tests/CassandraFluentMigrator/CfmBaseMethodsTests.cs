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
public class CfmBaseMethodsTests : IClassFixture<CassandraMigratorFixture>
{
    private const string TABLE_NOT_FOUND = "the table [{0}], was not found in the specified cassandra";
    private const string UDT_NOT_FOUND = "the User-Defined type [{0}], was not found in the specified cassandra";
    private const string INVALID_OR_EMPTY_ARGUMENT = "The argument [{0}] provided is either Null or Empty string";
    private const string INVALID_ARGUMENT = "The argument [{0}] provided is null";


    private const string COLUMN_NOT_FOUND =
            "the field [{0}] was not found in the specified type [{1}]. add the field in the object or check the field spelling!";

    private readonly CassandraMigratorFixture fixture;
    private readonly string NAME_OF_TABLE = nameof(CfmHelperObject).ToLower();

    public CfmBaseMethodsTests(CassandraMigratorFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Priority(0)]
    public async Task Initialize()
    {
        // Ensure that the Table we want to tests exists.
        await fixture.MigratorHelper
                .GetTable<CfmHelperObject>()
                .CreateIfNotExistsAsync();

        // Ensure that the UDT wa want to test exists.
        await fixture.MigratorHelper.CreateUserDefinedTypeAsync<CfmHelperObject>();
        Assert.True(true);

        /*
         * TODO: Test methods - Create Materialized view.
         * Ensure that the Materialized View and columns exists.
         * Ticket: https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/projects/1#card-39240284
         */
    }

    [Fact]
    [Priority(1)]
    public void DoesTableExists_Success()
    {
        var result = fixture.MigratorHelper.DoesTableExists(nameof(CfmHelperObject));
        Assert.True(result);
    }

    [Fact]
    [Priority(1)]
    public void DoesTableExists_NotFound_Failed()
    {
        var result = fixture.MigratorHelper.DoesTableExists("DoesntExists");
        Assert.False(result);
    }

    [Fact]
    [Priority(2)]
    public void DoesColumnExists_Success()
    {
        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), "Values");
        Assert.True(result);
    }

    [Fact]
    [Priority(2)]
    public void DoesColumnExists_NotFound_Failed()
    {
        var result = fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), "DoesntExists");
        Assert.False(result);
    }

    [Fact]
    [Priority(2)]
    public void DoesColumnExists_TableNotFound_Failed()
    {
        try
        {
            fixture.MigratorHelper.DoesColumnExists("DoesntExists", "Values");
        }
        catch (ObjectNotFoundException ex)
        {
            var expected = string.Format(TABLE_NOT_FOUND, "DoesntExists").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(2)]
    public void DoesTable_ColumnExists_InvalidArguments_Failed()
    {
        try
        {
            fixture.MigratorHelper.DoesColumnExists(null, "Values");
        }
        catch (NullReferenceException ex)
        {
            var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "table").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }

        try
        {
            fixture.MigratorHelper.DoesColumnExists(nameof(CfmHelperObject), string.Empty);
        }
        catch (NullReferenceException ex)
        {
            var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "column").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(3)]
    public void DoesUdtExists_Success()
    {
        var result = fixture.MigratorHelper.DoesUdtExists(nameof(CfmHelperObject));
        Assert.True(result);
    }

    [Fact]
    [Priority(3)]
    public void DoesUdtExists_NotFound_Failed()
    {
        var result = fixture.MigratorHelper.DoesUdtExists("DoesntExists");
        Assert.False(result);
    }

    [Fact]
    [Priority(3)]
    public void DoesUdtExists_InvalidArguments_Failed()
    {
        try
        {
            fixture.MigratorHelper.DoesUdtExists(null);
        }
        catch (NullReferenceException ex)
        {
            var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "udt").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }

        try
        {
            ICassandraFluentMigrator tempHelper = null;
            tempHelper.DoesUdtExists(null);
        }
        catch (NullReferenceException ex)
        {
            Assert.Contains("Object reference not set to an instance of an object.".ToLower(), ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(4)]
    public void DoesUdt_ColumnExists_Success()
    {
        var result =
                fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), nameof(CfmHelperObject.Values));
        Assert.True(result);
    }

    [Fact]
    [Priority(4)]
    public void DoesUdt_ColumnExists_NotFound_Failed()
    {
        var result = fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), "DoesntExists");
        Assert.False(result);
    }

    [Fact]
    [Priority(4)]
    public void DoesUdt_ColumnExists_UdtNotFound_Failed()
    {
        try
        {
            fixture.MigratorHelper.DoesUdtColumnExists("DoesntExists", "Values");
        }
        catch (ObjectNotFoundException ex)
        {
            var expected = string.Format(UDT_NOT_FOUND, "DoesntExists").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(4)]
    public void DoesUdt_columnExists_InvalidArguments_Failed()
    {
        try
        {
            fixture.MigratorHelper.DoesUdtColumnExists(null, "Values");
        }
        catch (NullReferenceException ex)
        {
            var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "udt").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }

        try
        {
            fixture.MigratorHelper.DoesUdtColumnExists(nameof(CfmHelperObject), string.Empty);
        }
        catch (NullReferenceException ex)
        {
            var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "column").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(10)]
    public void GetCqlType_Success()
    {
        var type = fixture.MigratorHelper.GetCqlType(typeof(string));
        Assert.Equal("text", type);

        type = fixture.MigratorHelper.GetCqlType(typeof(CfmHelperObject), true);
        Assert.Equal($"frozen<{NAME_OF_TABLE}>", type);
    }

    [Fact]
    [Priority(10)]
    public void GetCqlType_InvalidType_Failed()
    {
        try
        {
            fixture.MigratorHelper.GetCqlType(null);
        }
        catch (NullReferenceException ex)
        {
            var expected = string.Format(INVALID_ARGUMENT, "type").ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }

    [Fact]
    [Priority(11)]
    public void GetColumnType_Success()
    {
        var result = fixture.MigratorHelper.GetColumnType<CfmHelperObject>("id");
        Assert.Equal("int", result);
    }

    [Fact]
    [Priority(11)]
    public void GetColumnType_Failed()
    {
        try
        {
            fixture.MigratorHelper.GetColumnType<CfmHelperObject>("DoesntExists");
        }
        catch (ObjectNotFoundException ex)
        {
            var expected = string.Format(COLUMN_NOT_FOUND, "DoesntExists", NAME_OF_TABLE).ToLower();
            Assert.Contains(expected, ex.Message.ToLower());
        }
    }
}