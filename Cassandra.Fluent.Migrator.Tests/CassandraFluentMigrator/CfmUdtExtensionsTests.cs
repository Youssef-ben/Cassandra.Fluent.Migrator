namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator
{
    using System;
    using Cassandra.Fluent.Migrator.Common.Configuration;
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Helper.Extensions;
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
    public class CfmUdtExtensionsTests
    {
        private const string UDT_NOT_FOUND = "the User-Defined type [{0}], was not found in the specified cassandra keyspace [{1}]!";
        private const string CANT_RENAME = "cannot add new field [{0}] to type!";
        private const string KEYSPACE = "udt_f1ddd43b_ad8d_4732_b623_bc65c539f04f";

        private readonly ISession session;
        private readonly ICassandraFluentMigrator cfmHelper;

        public CfmUdtExtensionsTests()
        {
            if (this.session is null)
            {
                this.session = this.GetTestCassandraSession(KEYSPACE);
                this.cfmHelper = new CassandraFluentMigrator(this.session);
            }
        }

        [Fact]
        [Priority(0)]
        public async void CreateUserDefinedType_Success()
        {
            var udtName = nameof(CfmHelperObject);

            var result = this.cfmHelper.DoesUdtExists(udtName);
            Assert.False(result);

            await this.cfmHelper.CreateUserDefinedTypeAsync<CfmHelperObject>();

            result = this.cfmHelper.DoesUdtExists(udtName);
            Assert.True(result);
        }

        [Fact]
        [Priority(1)]
        public async void DeleteTheUserDefinedType_Success()
        {
            var udtName = nameof(CfmHelperObject);

            var result = this.cfmHelper.DoesUdtExists(udtName);
            Assert.True(result);

            await this.cfmHelper.DeleteUserDefinedTypeAsync<CfmHelperObject>();

            result = this.cfmHelper.DoesUdtExists(udtName);
            Assert.False(result);
        }

        [Fact]
        [Priority(2)]
        public async void Initialize_ForTheRestOfTests()
        {
            var result = this.cfmHelper.DoesUdtExists(nameof(CfmHelperObject));
            Assert.False(result);

            /*
             * Ensure that the UDT we want to create exists.
             * This was created based on the object {CfmHelperObject} that normally contains 3 field,
             * but in this method we created only 2 fields for testing purposes.
             */
            IStatement statement = new SimpleStatement($"CREATE TYPE IF NOT EXISTS {nameof(CfmHelperObject)} (id int, values text)");
            await this.session.ExecuteAsync(statement);

            result = this.cfmHelper.DoesUdtExists(nameof(CfmHelperObject));
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public async void AddColumn_Success()
        {
            var column = "AddedColumnFromTest";

            var result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
            Assert.False(result);

            await this.cfmHelper.AlterUdtAddColumnAsync(nameof(CfmHelperObject), column, typeof(bool));

            result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public async void AddColumn_WithoutType_Success()
        {
            var column = nameof(CfmHelperObject.AddedColumnFromTestwithoutType);

            var result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
            Assert.False(result);

            await this.cfmHelper.AlterUdtAddColumnAsync<CfmHelperObject>(nameof(CfmHelperObject), column);

            result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
            Assert.True(result);
        }

        [Fact]
        [Priority(3)]
        public async void AddColumn_UdtNotFound_Failed()
        {
            try
            {
                await this.cfmHelper.AlterUdtAddColumnAsync("DoesntExists", "Values", typeof(string));
            }
            catch (ObjectNotFoundException ex)
            {
                var expected = string.Format(UDT_NOT_FOUND, "DoesntExists", KEYSPACE).ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }
        }

        [Fact]
        [Priority(4)]
        public async void RenameColumn_Success()
        {
            var column = "values";
            var target = "valuesDetails";

            var result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
            Assert.True(result);

            result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), target);
            Assert.False(result);

            // Execute
            await this.cfmHelper.AlterUdtRenameColumnAsync(nameof(CfmHelperObject), column, target);

            result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), column);
            Assert.False(result);

            result = this.cfmHelper.DoesUdtColumnExists(nameof(CfmHelperObject), target);
            Assert.True(result);
        }

        [Fact]
        [Priority(4)]
        public async void RenameColumn_TargetColumnExists_Failed()
        {
            try
            {
                await this.cfmHelper.AlterUdtRenameColumnAsync(nameof(CfmHelperObject), "AddedColumnFromTest", "id");
            }
            catch (InvalidOperationException ex)
            {
                var expected = string.Format(CANT_RENAME, "id").ToLower();
                Assert.Contains(expected, ex.Message.ToLower());
            }
        }

        [Fact]
        [Priority(4)]
        public async void RenameColumn_UdtNotFound_Failed()
        {
            try
            {
                await this.cfmHelper.AlterUdtRenameColumnAsync("DoesntExists", "Values", "valuesDetails");
            }
            catch (ObjectNotFoundException ex)
            {
                var expected = string.Format(UDT_NOT_FOUND, "DoesntExists", KEYSPACE).ToLower();
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
