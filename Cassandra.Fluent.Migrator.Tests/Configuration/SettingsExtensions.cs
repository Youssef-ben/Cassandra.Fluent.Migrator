namespace Cassandra.Fluent.Migrator.Tests.Configuration;

using Common.Models.Configuration;
using Microsoft.Extensions.Configuration;

public static class SettingsExtensions
{
    /// <summary>
    ///     Validate the Cassandra configuration section and return a new instance of the Cassandra Setting object.
    ///     (Target Tests).
    /// </summary>
    /// <returns>New Instance of the Cassandra settings.</returns>
    internal static CassandraSettings GetCassandraSettings()
    {
        var sectionName = nameof(CassandraSettings).Replace("Settings", string.Empty);
        return LoadConfiguration()
                .GetConfigInstance(sectionName)
                .ValidateSetting();
    }

    /// <summary>
    ///     Convert the specified configuration section into an object.
    /// </summary>
    /// <param name="self">The Configuration Object.</param>
    /// <param name="section">The desired section that we want to convert into an object.</param>
    /// <returns>New Cassandra settings instance.</returns>
    private static CassandraSettings GetConfigInstance(this IConfiguration self, string section)
    {
        var instance = new CassandraSettings();
        self.Bind(section, instance);
        return instance;
    }

    /// <summary>
    ///     Load the appsettings Configuration file in memory.
    /// </summary>
    /// <returns>Appsettings Configuration.</returns>
    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
                .AddJsonFile("Settings/appsettings.json", false)
                .Build();
    }
}