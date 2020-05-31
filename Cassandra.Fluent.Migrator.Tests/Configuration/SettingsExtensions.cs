﻿namespace Cassandra.Fluent.Migrator.Tests.Configuration
{
    using System;
    using Cassandra.Fluent.Migrator.Tests.Models.Configuration;
    using Microsoft.Extensions.Configuration;

    public static class SettingsExtensions
    {
        /// <summary>
        /// Validate the Cassandra configuration section and return a new instance of the Cassandra Setting object.
        /// </summary>
        /// <returns>New Instance of the Cassandra settings.</returns>
        public static CassandraSettings GetConfiguration()
        {
            return LoadConfiguration()
                .GetConfigInstance(nameof(CassandraSettings).Replace("Settings", string.Empty))
                .ValidateConfiguration();
        }

        /// <summary>
        /// Validate the Cassandra Configuration file.
        /// </summary>
        /// <param name="self">The Cassandra configuration Instance.</param>
        /// <returns>Self if valid.</returns>
        private static CassandraSettings ValidateConfiguration(this CassandraSettings self)
        {
            var argumentNullExceptionMessage = "The configuration [{0}] section is invalid";

            if (self.ContactPoints is null || self.ContactPoints.Count == 0)
            {
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, "Contact Point"));
            }

            if (self.Port == 0)
            {
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, "Port"));
            }

            if (self.Credentials is null || string.IsNullOrWhiteSpace(self.Credentials.Username) || string.IsNullOrWhiteSpace(self.Credentials.Password))
            {
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, "Credentials"));
            }

            if (string.IsNullOrWhiteSpace(self.DefaultKeyspace))
            {
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, "Default Keyspace"));
            }

            if (self.Replication is null || string.IsNullOrWhiteSpace(self.Replication["class"]) || string.IsNullOrWhiteSpace(self.Replication["datacenter"]))
            {
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, "Replication"));
            }

            if (self.Query is null || self.Query.HeartBeat == 0)
            {
                throw new ArgumentNullException(string.Format(argumentNullExceptionMessage, "Query"));
            }

            return self;
        }

        /// <summary>
        /// Convert the specified configuration section into an object.
        /// </summary>
        /// <param name="self">The Configuration Object.</param>
        /// <param name="section">The desired section that we want to convert into an object.</param>
        /// <returns>New Cassandra setitngs instance.</returns>
        private static CassandraSettings GetConfigInstance(this IConfiguration self, string section)
        {
            var instance = new CassandraSettings();
            self.Bind(section, instance);
            return instance;
        }

        /// <summary>
        /// Load the appsettings Configuration file in memory.
        /// </summary>
        /// <returns>Appsettings Configuration.</returns>
        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
            .AddJsonFile("settings/appsettings.json", false)
            .AddJsonFile("settings/appsettings.local.json", optional: true)
            .AddJsonFile("settings/appsettings.development.json", optional: true)
            .AddJsonFile("settings/appsettings.tests.json", optional: true)
            .Build();
        }
    }
}
