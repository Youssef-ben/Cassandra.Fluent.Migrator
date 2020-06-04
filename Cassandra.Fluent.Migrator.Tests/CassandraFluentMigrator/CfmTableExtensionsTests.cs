namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator
{
    using System;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Helper.Extensions;
    using Cassandra.Fluent.Migrator.Tests.Configuration;
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
    public class CfmTableExtensionsTests
    {
        private const string KEYSPACE = "tables_aed8344d_2a09_44f4_aa0d_f1b6ead51c6a";

        private readonly ISession session;
        private readonly ICassandraFluentMigrator cfmHelper;

        public CfmTableExtensionsTests()
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
            var result = this.cfmHelper.DoesTableExists(nameof(CfmHelperObject));
            Assert.False(result);

            /*
             * Ensure that the Table we want to create exists.
             * This was created based on the object {CfmHelperObject} that normally contains 3 field,
             * but in this method we created only 2 fields for testing purposes.
             */
            IStatement statement = new SimpleStatement($"CREATE TABLE IF NOT EXISTS {nameof(CfmHelperObject)}(id int, values text, PRIMARY KEY (id))");
            await this.session.ExecuteAsync(statement);

            result = this.cfmHelper.DoesTableExists(nameof(CfmHelperObject));
            Assert.True(result);
        }

        [Fact]
        [Priority(1)]
        public async void AddColumn_TypeSpecified_Success()
        {
            var column = "AddedColumnFromTest";

            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), column);
            Assert.False(result);

            await this.cfmHelper.AddColumnAsync(nameof(CfmHelperObject), column, typeof(string));

            result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), column);
            Assert.True(result);
        }

        [Fact]
        [Priority(1)]
        public async void AddColumn_TypeNotSpecified_Success()
        {
            var column = "AddedColumnFromTestwithoutType";

            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), column);
            Assert.False(result);

            await this.cfmHelper.AddColumnAsync<CfmHelperObject>(nameof(CfmHelperObject), column);

            result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), column);
            Assert.True(result);
        }

        [Fact]
        [Priority(2)]
        public async void RenamePrimaryKey_Success()
        {
            var old = "id";
            var target = "renamedId";

            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), old);
            Assert.True(result);

            result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), target);
            Assert.False(result);

            await this.cfmHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), old, target);

            result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), old);
            Assert.False(result);

            result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), target);
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public async void RenamePrimaryKey_DoesntExists_SKIPPED()
        {
            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), "id");
            Assert.False(result);

            await this.cfmHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), "id", "renamedId");
        }

        [Fact]
        [Priority(4)]
        public async void RenamePrimaryKey_ColumnNotPrimary_Failed()
        {
            try
            {
                await this.cfmHelper.RenamePrimaryColumnAsync(nameof(CfmHelperObject), "Values", "renamedId");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains("the [values] is not a primary key. you can only rename primary keys!".ToLower(), ex.Message.ToLower());
            }
        }

        [Fact]
        [Priority(5)]
        public async void DeleteColumn_Success()
        {
            var column = "AddedColumnFromTestwithoutType";

            var result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), column);
            Assert.True(result);

            await this.cfmHelper.DropColumnAsync(nameof(CfmHelperObject), column);

            result = this.cfmHelper.DoesColumnExists(nameof(CfmHelperObject), column);
            Assert.False(result);
        }

        [Fact]
        [Priority(5)]
        public async void DeleteColumn_Failed()
        {
            try
            {
                await this.cfmHelper.DropColumnAsync("TableDoesntExists", "AddedColumnFromTestwithoutType");
            }
            catch (ObjectNotFoundException ex)
            {
                Assert.Contains("the table [tabledoesntexists], was not found in the specified cassandra keyspace [tables_aed8344d_2a09_44f4_aa0d_f1b6ead51c6a]!".ToLower(), ex.Message.ToLower());
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
