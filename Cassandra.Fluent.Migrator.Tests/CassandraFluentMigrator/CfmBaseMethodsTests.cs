namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator
{
    using System;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Tests.Configuration;
    using Cassandra.Fluent.Migrator.Tests.Models;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Xunit;
    using Xunit.Priority;

    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class CfmBaseMethodsTests
    {
        private const string TABLE_NOT_FOUND = "the table [{0}], was not found in the specified cassandra keyspace [{1}]!";
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
                this.session = this.GetCassandraSession(KEYSPACE);
                this.cfmHelper = new CassandraFluentMigrator(this.session);
            }
        }

        [Fact]
        [Priority(0)]
        public async void Initialize()
        {
            // Ensure that the Table we want to create exists
            await this.cfmHelper
                .GetTable<CfmHelperObject>()
                .CreateIfNotExistsAsync();

            /*
             * TODO: Test methods - Create UDT.
             * Ensure that the User-Defined type and columns exists.
             * Ticket: https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/projects/1#card-39240226
             *
             * TODO: Test methods - Create Materialized view.
             * Ensure that the Materialized View and columns exists.
             * Ticket: https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/projects/1#card-39240284
             */

            await this.cfmHelper.GetCassandraSession()
                .UserDefinedTypes.DefineAsync(
                    UdtMap
                    .For<CfmHelperObject>()
                    .Automap());
        }

        [Fact]
        [Priority(1)]
        public void DoesTableExists_Sucess()
        {
            var result = this.cfmHelper.DoesTableExists(nameof(CfmHelperObject));
            Assert.True(result);
        }

        [Fact]
        [Priority(1)]
        public void DoesTableExists_Failed_NotFound()
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
        public void DoesColumnExists_Failed_NotFound()
        {
            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), "DoesntExists");
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public void DoesColumnExists_Failed_TableNotFound()
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
        public void DoesTable_column_exists_Invalid_arguments()
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
        [Priority(10)]
        public void GetCqlType_Success()
        {
            var type = this.cfmHelper.GetCqlType(typeof(string));
            Assert.Equal("text", type);

            type = this.cfmHelper.GetCqlType(typeof(CfmHelperObject));
            Assert.Equal("frozen<cfmhelperobject>", type);
        }

        [Fact]
        [Priority(10)]
        public void GetCqlType_Failed_InvalidType()
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
        public async void DeleteKeyspace_and_ShutdownTheSession()
        {
            this.session.DeleteKeyspaceIfExists(KEYSPACE);

            await this.session.ShutdownAsync();
        }
    }
}
