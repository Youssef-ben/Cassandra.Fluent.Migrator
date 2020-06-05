namespace Cassandra.Fluent.Migrator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Cassandra.Data.Linq;
    using Cassandra.Fluent.Migrator.Core.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class CassandraMigrator : ICassandraMigrator
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<CassandraMigrator> logger;
        private readonly ISession cassandraSession;
        private readonly string keyspace;

        private readonly Table<MigrationHistory> migrationHistory;

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
            this.logger.LogDebug("Making sure that the migration history table exusts in the keyspace...");
            this.migrationHistory = new Table<MigrationHistory>(this.cassandraSession);
            this.migrationHistory.CreateIfNotExists();
        }

        public MigrationHistory GetLatestMigration()
        {
            throw new NotImplementedException();
        }

        public Task<MigrationHistory> GetLatestMigrationAsync()
        {
            throw new NotImplementedException();
        }

        public ICollection<IMigrator> GetMigrations()
        {
            this.logger.LogDebug("Fetching the registred migrations from the internal service provider...");
            var migrations = this.serviceProvider.GetService<IEnumerable<IMigrator>>();

            if (migrations is null)
            {
                migrations = new List<IMigrator>();
            }

            return migrations.ToList();
        }

        public Task<ICollection<MigrationHistory>> GetMigrationsAsync()
        {
            throw new NotImplementedException();
        }

        public ICollection<MigrationHistory> GetAppliedMigration()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<MigrationHistory>> GetAppliedMigrationAsync()
        {
            throw new NotImplementedException();
        }

        Task<MigrationHistory> ICassandraMigrator.GetLatestMigrationAsync()
        {
            throw new NotImplementedException();
        }

        MigrationHistory ICassandraMigrator.GetLatestMigration()
        {
            throw new NotImplementedException();
        }

        void ICassandraMigrator.Migrate()
        {
            this.logger.LogInformation("Starting the migration process.");

            var migrations = this.GetMigrations();

            foreach (var migration in migrations)
            {
                migration.ApplyMigration();
            }
        }
    }
}
