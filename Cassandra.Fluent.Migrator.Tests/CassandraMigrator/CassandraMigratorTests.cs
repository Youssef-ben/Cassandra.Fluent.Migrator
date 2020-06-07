namespace Cassandra.Fluent.Migrator.Tests.CassandraMigrator
{
    using System;
    using System.Linq;
    using Cassandra.Fluent.Migrator.Common.Configuration;
    using Cassandra.Fluent.Migrator.Common.Models;
    using Cassandra.Fluent.Migrator.Core;
    using Cassandra.Fluent.Migrator.Helper;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Priority;

    /// <summary>
    /// IMPORTANT NOTE: the use of {nameof(...)} in this test file is only
    /// to make sure that we have a consistency and the tests don't break
    /// BUT IN REAL WORLD application use MUST use a "string" for the migrations.
    /// </summary>
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class CassandraMigratorTests
    {
        private readonly ICassandraMigrator migrator;
        private readonly ICassandraFluentMigrator cfm;

        public CassandraMigratorTests()
        {
            if (this.migrator is null)
            {
                var serviceProvider = this.GetTestInMemoryProvider();
                var logger = serviceProvider.GetTestService<ILogger<CassandraMigrator>>();

                this.migrator = new CassandraMigrator(serviceProvider, logger);
                this.cfm = serviceProvider.GetTestService<ICassandraFluentMigrator>();
            }
        }

        [Fact]
        [Priority(0)]
        public void GetRegistredMigrations_Success()
        {
            var result = this.migrator.GetRegistredMigrations();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(new Version(1, 0, 0), result.FirstOrDefault().Version);
        }

        [Fact]
        [Priority(1)]
        public void GetAppliedMigration_Empty_Success()
        {
            var result = this.migrator.GetAppliedMigrations();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        [Priority(2)]
        public void GetLatestMigration_Null_Success()
        {
            var result = this.migrator.GetLatestMigration();

            Assert.Null(result);
        }

        [Fact]
        [Priority(3)]
        public void MigrateSchema_Success()
        {
            var result = this.migrator.Migrate();

            Assert.Equal(1, result);

            Assert.True(this.cfm.DoesTableExists(nameof(Users)));
            Assert.True(this.cfm.DoesUdtExists(nameof(Address)));
        }

        [Fact]
        [Priority(4)]
        public void MigrateSchema_Nothing_Should_be_applied_Success()
        {
            var result = this.migrator.Migrate();

            Assert.Equal(0, result);
        }

        [Fact]
        [Priority(5)]
        public void GetAppliedMigration_Success()
        {
            var result = this.migrator.GetAppliedMigrations();

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.Equal("1.0.0", result.FirstOrDefault().Version);
        }

        [Fact]
        [Priority(5)]
        public void GetLatestMigration_Success()
        {
            var result = this.migrator.GetLatestMigration();

            Assert.NotNull(result);
            Assert.Equal("1.0.0", result.Version);
        }

        [Fact]
        [Priority(100)]
        public async void RemoveKeyspace_and_ShutdownTheSession()
        {
            var session = this.cfm.GetCassandraSession();
            session.DeleteKeyspaceIfExists(session.Keyspace);
            await session.ShutdownAsync();
        }
    }
}
