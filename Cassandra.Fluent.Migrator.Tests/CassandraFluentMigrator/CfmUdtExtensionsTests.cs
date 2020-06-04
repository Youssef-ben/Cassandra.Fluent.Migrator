namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator
{
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Helper.Extensions;
    using Cassandra.Fluent.Migrator.Tests.Configuration;
    using Cassandra.Fluent.Migrator.Tests.Models;
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
        private const string KEYSPACE = "udt_f1ddd43b_ad8d_4732_b623_bc65c539f04f";

        private readonly ISession session;
        private readonly ICassandraFluentMigrator cfmHelper;

        public CfmUdtExtensionsTests()
        {
            if (this.session is null)
            {
                this.session = this.GetCassandraSession(KEYSPACE);
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
        [Priority(100)]
        public async void DeleteKeyspace_and_ShutdownTheSession()
        {
            this.session.DeleteKeyspaceIfExists(KEYSPACE);
            await this.session.ShutdownAsync();
        }
    }
}
