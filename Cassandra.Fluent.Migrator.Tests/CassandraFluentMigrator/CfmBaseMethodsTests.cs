namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator
{
    using System;
    using Cassandra.Fluent.Migrator.Common.Configuration;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Tests.Models;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Xunit;
    using Xunit.Priority;

    /// <summary>
    /// IMPORTANT NOTE: the use of {nameof(...)} in this test file is only
    /// to make sure that we have a consistency and the tests don't break
    /// BUT IN REAL WORLD application use MUST use a "string" for the migrations.
    /// </summary>
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class CfmBaseMethodsTests
    {
        private const string TABLE_NOT_FOUND = "the table [{0}], was not found in the specified cassandra keyspace [{1}]!";
        private const string UDT_NOT_FOUND = "the User-Defined type [{0}], was not found in the specified cassandra keyspace [{1}]!";
        private const string COLUMN_NOT_FOUND = "the field [{0}] was not found in the specified type [{1}]. add the field in the object or check the field spelling!";
        private const string INVALID_OR_EMPTY_ARGUMENT = "The argument [{0}] provided is either Null or Empty string";
        private const string INVALID_ARGUMENT = "The argument [{0}] provided is null";
        private const string KEYSPACE = "base_740ee67a_4718_4117_8d3d_6f50c6343f48";

        private readonly ISession session;
        private readonly ICassandraFluentMigrator cfmHelper;

        public CfmBaseMethodsTests()
        {
            if (this.session is null)
            {
                this.session = this.GetTestCassandraSession(KEYSPACE);
                this.cfmHelper = new CassandraFluentMigrator(this.session);
            }
        }

        [Fact]
        [Priority(0)]
        public async void Initialize()
        {
            // Ensure that the Table we want to tests exists.
            await this.cfmHelper
                .GetTable<CfmHelperObject>()
                .CreateIfNotExistsAsync();

            // Ensure that the UDT wa want to test exists.
            await this.cfmHelper.CreateUserDefinedTypeAsync<CfmHelperObject>();

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
            var result = this.cfmHelper.DoesTableExists(nameof(CfmHelperObject));
            Assert.True(result);
        }

        [Fact]
        [Priority(1)]
        public void DoesTableExists_NotFound_Failed()
        {
            var result = this.cfmHelper.DoesTableExists("DoesntExists");
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public void DoesColumnExists_Success()
        {
            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), "Values");
            Assert.True(result);
        }

        [Fact]
        [Priority(2)]
        public void DoesColumnExists_NotFound_Failed()
        {
            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), "DoesntExists");
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public void DoesColumnExists_TableNotFound_Failed()
        {
            try
            {
                this.cfmHelper.DoesColumnExists("DoesntExists", "Values");
            }
            catch (ObjectNotFoundException ex)
            {
                var expected = string.Format(TABLE_NOT_FOUND, "DoesntExists", KEYSPACE).ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }
        }

        [Fact]
        [Priority(2)]
        public void DoesTable_ColumnExists_InvalidArguments_Failed()
        {
            try
            {
                this.cfmHelper.DoesColumnExists(null, "Values");
            }
            catch (NullReferenceException ex)
            {
                var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "table").ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }

            try
            {
                this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), string.Empty);
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
            var result = this.cfmHelper.DoesUdtExists(nameof(CfmHelperObject));
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public void DoesUdtExists_NotFound_Failed()
        {
            var result = this.cfmHelper.DoesUdtExists("DoesntExists");
            Assert.False(result);
        }

        [Fact]
        [Priority(3)]
        public void DoesUdtExists_InvalidArguments_Failed()
        {
            try
            {
                this.cfmHelper.DoesUdtExists(null);
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
            var result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), nameof(CfmHelperObject.Values));
            Assert.True(result);
        }

        [Fact]
        [Priority(4)]
        public void DoesUdt_ColumnExists_NotFound_Failed()
        {
            var result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), "DoesntExists");
            Assert.False(result);
        }

        [Fact]
        [Priority(4)]
        public void DoesUdt_ColumnExists_UdtNotFound_Failed()
        {
            try
            {
                this.cfmHelper.DoesUdtColumnExists("DoesntExists", "Values");
            }
            catch (ObjectNotFoundException ex)
            {
                var expected = string.Format(UDT_NOT_FOUND, "DoesntExists", KEYSPACE).ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }
        }

        [Fact]
        [Priority(4)]
        public void DoesUdt_columnExists_InvalidArguments_Failed()
        {
            try
            {
                this.cfmHelper.DoesUdtColumnExists(null, "Values");
            }
            catch (NullReferenceException ex)
            {
                var expected = string.Format(INVALID_OR_EMPTY_ARGUMENT, "udt").ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }

            try
            {
                this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), string.Empty);
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
            var type = this.cfmHelper.GetCqlType(typeof(string));
            Assert.Equal("text", type);

            type = this.cfmHelper.GetCqlType(typeof(CfmHelperObject), true);
            Assert.Equal("frozen<cfmhelperobject>", type);
        }

        [Fact]
        [Priority(10)]
        public void GetCqlType_InvalidType_Failed()
        {
            try
            {
                this.cfmHelper.GetCqlType(null);
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
            var result = this.cfmHelper.GetColumnType<CfmHelperObject>("id");
            Assert.Equal("int", result);
        }

        [Fact]
        [Priority(11)]
        public void GetColumnType_Failed()
        {
            try
            {
                this.cfmHelper.GetColumnType<CfmHelperObject>("DoesntExists");
            }
            catch (ObjectNotFoundException ex)
            {
                var expected = string.Format(COLUMN_NOT_FOUND, "DoesntExists", "cfmhelperobject").ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }
        }

        [Fact]
        [Priority(100)]
        public async void RemoveKeyspace_and_ShutdownTheSession()
        {
            this.session.DeleteKeyspaceIfExists(KEYSPACE);

            await this.session.ShutdownAsync();
        }
    }
}
