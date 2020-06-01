namespace Cassandra.Fluent.Migrator.Tests.CassandraFluentMigrator
{
    using Cassandra.Fluent.Migrator.Helper;
    using Cassandra.Fluent.Migrator.Helper.Extensions;
    using Cassandra.Fluent.Migrator.Tests.Configuration;
    using Cassandra.Fluent.Migrator.Tests.Models;
    using Xunit;
    using Xunit.Priority;

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
            // Ensure that the Table we want to create exists.
            IStatement statement = new SimpleStatement("CREATE TABLE IF NOT EXISTS CfmHelperObject(id int, values text, PRIMARY KEY (id))");
            await this.session.ExecuteAsync(statement);
        }

        [Fact]
        [Priority(1)]
        public async void AddColumn_TypeSpecified_Success()
        {
            await this.cfmHelper.AddColumnAsync("CfmHelperObject", "AddedColumnFromTest", typeof(string));
        }

        [Fact]
        [Priority(1)]
        public async void AddColumn_TypeNotSpecified_Success()
        {
            await this.cfmHelper.AddColumnAsync<CfmHelperObject>("AddedColumnFromTestwithoutType");
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
