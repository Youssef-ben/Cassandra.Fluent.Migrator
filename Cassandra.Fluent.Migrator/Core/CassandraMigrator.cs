namespace Cassandra.Fluent.Migrator.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Linq;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Models;
    using Utils.Exceptions;

    public class CassandraMigrator : ICassandraMigrator
    {
        private readonly ILogger<CassandraMigrator> logger;
        private readonly Table<MigrationHistory> migrationHistory;
        private readonly IServiceProvider serviceProvider;

        public CassandraMigrator(IServiceProvider serviceProvider, ILogger<CassandraMigrator> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;

            this.logger.LogInformation("Started the initializing process of the Cassandra Migrator!");

            var cassandraSession = this.serviceProvider.GetService<ISession>();
            if (cassandraSession is null)
            {
                var message = "The Cassandra session was not found in the service provider!";
                message += "Please try registering the cassandra session before!";
                throw new ObjectNotFoundException(message);
            }

            // Ensure that Keyspace exists.
            this.logger.LogDebug("Making sure that the keyspace exists...");
            cassandraSession.CreateKeyspaceIfNotExists(cassandraSession.Keyspace);

            // Ensure that the Migration table exists.
            this.logger.LogDebug("Making sure that the migration history table exists in the keyspace!");
            migrationHistory = new Table<MigrationHistory>(cassandraSession);
            migrationHistory.CreateIfNotExists();

            this.logger.LogInformation("The Cassandra Migrator initializing process finished!");
        }

        public ICollection<IMigrator> GetRegisteredMigrations()
        {
            logger.LogInformation("Fetching the registered migrations from the service provider!");
            List<IMigrator> migrations = serviceProvider
                    .GetService<IEnumerable<IMigrator>>()
                    .ToList();

            logger.LogInformation("Found ({Count}) registered migration(s) in the service provider!",
                    migrations.Count);

            logger.LogDebug("Ordering the registered migrations by version!");
            return migrations
                    .OrderBy(x => x.Version)
                    .ToList();
        }

        public ICollection<MigrationHistory> GetAppliedMigrations()
        {
            logger.LogInformation("Fetching the applied migrations from the the database!");
            ICollection<MigrationHistory> result = migrationHistory
                    .Execute()
                    .ToList();

            logger.LogInformation("Found ({Count}) applied migration(s)!", result.Count);
            if (result.Count == 0)
            {
                return result;
            }

            logger.LogDebug("Ordering the applied migrations by version!");
            return result
                    .OrderByDescending(x => x.Version)
                    .ToList();
        }

        public MigrationHistory GetLatestMigration()
        {
            ICollection<MigrationHistory> appliedMigrations = GetAppliedMigrations();

            logger.LogInformation("Fetching the last applied migration from the database!");
            MigrationHistory latestMigration = appliedMigrations.FirstOrDefault();
            if (latestMigration is null)
            {
                logger.LogWarning("Couldn't find any applied migration!");
                return default;
            }

            logger.LogInformation("The last applied migration is ([{Version}] - [{Name}])!",
                    latestMigration.Version,
                    latestMigration.Name);
            return latestMigration;
        }

        public int Migrate()
        {
            logger.LogInformation("Starting the migration process!");
            ICollection<IMigrator> registeredMigrations = GetRegisteredMigrations();
            MigrationHistory latestAppliedMigration = GetLatestMigration();
            Version latestVersion = latestAppliedMigration is null
                                            ? default
                                            : new Version(latestAppliedMigration.Version);

            var appliedMigrationsCount = 0;
            foreach (IMigrator migration in registeredMigrations)
            {
                logger.LogDebug("Checking if the migration [{Name}] should be applied!", migration.Name);
                if (latestVersion != null && migration.Version <= latestVersion)
                {
                    logger.LogWarning("[SKIPPING] - The migration [{Name}] is already applied!", migration.Name);
                    continue;
                }

                logger.LogInformation("Applying the migration [{Name}]!", migration.Name);
                migration.ApplyMigrationAsync().GetAwaiter().GetResult();

                logger.LogDebug("Migration applied! Updating the migration history");
                this.UpdateMigrationHistory(migrationHistory, migration);

                appliedMigrationsCount++;
            }

            logger.LogInformation(
                    "The Migration process is done with ({Count}) Migration(s) applied !",
                    appliedMigrationsCount);
            return appliedMigrationsCount;
        }
    }
}