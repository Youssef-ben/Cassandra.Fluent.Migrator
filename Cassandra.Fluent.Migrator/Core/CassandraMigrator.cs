namespace Cassandra.Fluent.Migrator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cassandra.Data.Linq;
    using Cassandra.Fluent.Migrator.Core.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CassandraMigrator : ICassandraMigrator
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<CassandraMigrator> logger;
        private readonly ISession cassandraSession;

        private readonly Table<MigrationHistory> migrationHistory;

        private readonly string keyspace;
        private bool showLog = true;

        public CassandraMigrator(IServiceProvider serviceProvider, ILogger<CassandraMigrator> logger, ISession session)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.cassandraSession = session;
            this.keyspace = this.cassandraSession.Keyspace;

            this.logger.LogInformation("Initializing the Cassandra Migrator.");

            // Ensure that Keyspace exists.
            this.logger.LogDebug("Making sure that the default keyspace exists...");
            this.cassandraSession.CreateKeyspaceIfNotExists(this.keyspace);

            // Ensure that the Migration table exists.
            this.logger.LogDebug("Making sure that the migration history table exists in the keyspace...");
            this.migrationHistory = new Table<MigrationHistory>(this.cassandraSession);
            this.migrationHistory.CreateIfNotExists();
        }

        public ICollection<IMigrator> GetRegistredMigrations()
        {
            this.logger.LogDebug("Fetching the registred migrations from the internal service provider...");
            var migrations = this.serviceProvider.GetService<IEnumerable<IMigrator>>();

            if (migrations is null)
            {
                this.logger.LogWarning("Couldn't find any migration to apply!");
                migrations = new List<IMigrator>();
            }

            return migrations
                .OrderBy(x => x.Version)
                .ToList();
        }

        public ICollection<MigrationHistory> GetAppliedMigration()
        {
            if (this.showLog)
            {
                this.logger.LogDebug("Fetching the applied migrations from the database...");
            }

            var result = this
                .migrationHistory
                .Execute()
                .ToList();

            if (!result.Any())
            {
                return new List<MigrationHistory>();
            }

            return result
                .OrderByDescending(x => x.Version)
                .ToList();
        }

        public MigrationHistory GetLatestMigration()
        {
            this.logger.LogDebug("Fetching the last applied migration from the database...");

            this.showLog = false;

            var migrations = this.GetAppliedMigration();

            this.showLog = true;

            return migrations.FirstOrDefault();
        }

        int ICassandraMigrator.Migrate()
        {
            this.logger.LogInformation("Starting the migration process.");
            var count = 0;

            foreach (var migration in this.GetRegistredMigrations())
            {
                this.logger.LogDebug($"Checking if the migration [{migration.Name}] should be applied...");
                if (!this.ShouldApplyMigration(migration))
                {
                    this.logger.LogWarning($"SKIPPING the migration [{migration.Name}], already applied, ...");
                    continue;
                }

                this.logger.LogDebug($"Executing the migration [{migration.Name}]...");
                migration.ApplyMigrationAsync().GetAwaiter().GetResult();

                this.logger.LogDebug("Updating the migration history....");
                this.UpdateMigrationHistory(this.migrationHistory, migration);

                count++;
            }

            this.logger.LogInformation($"The Migration process is done. Migration(s) applied [{count}].");
            return count;
        }
    }
}
