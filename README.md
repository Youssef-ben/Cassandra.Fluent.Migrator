# Cassandra Fluent Migrator

![Cassandra Fluent library](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/workflows/Cassandra%20Fluent%20library/badge.svg?branch=dev)

Cassandra Fluent Migrator is a library that offers a set of fluent code and extensions to facilitate the creation and management of the migrations using code instead of CQL commands.

## Stack

* [.NET Standard 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?WT.mc_id=dotnet-35129-website&tabs=net-standard-2-1)
* [Cassandra CSharp driver - v3.x.x](https://docs.datastax.com/en/developer/csharp-driver/3.16/): A modern, feature-rich and highly tunable C# client library for Apache Cassandra using Cassandra’s binary protocol and Cassandra Query Language v3.

### Installation

* [Get it from NuGet](https://www.nuget.org/packages/Cassandra.Fluent.Migrator/)

```cmd
PM> Install-Package Cassandra.Fluent.Migrator
```

### Future Improvements

* [ ] Add support for more complex types.
* [ ] Add support for the `Materialized views`.

### Documentations

* [Wiki page:](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/wiki) Full documentation for the library.
* [Cassandra Migrator:](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/wiki/Cassandra-Migrator) The core class of the library.
* [Cassandra Fluent Migrator:](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/wiki/Cassandra-Fluent-Migrator) The migration helper class.
* [Migrator interface:](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/wiki/Migrator-Interface) The base properties and method that the migrations should implement.
* [Example:](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/wiki/Example) An example on how to use the library.
* [Supported Types:](https://github.com/Youssef-ben/Cassandra.Fluent.Migrator/wiki/Supported-Types) List of supported types.

### Basic Usage

* **Migration class**

In your project create a migration class that implements the `IMigrator` Interface as follow:

```C#
public class InitialMigration : IMigrator
{
    private readonly ICassandraFluentMigrator cfm;
    private readonly ILogger<InitialMigration> logger;

    public InitialMigration(ILogger<InitialMigration> logger, ICassandraFluentMigrator cfm)
    {
        this.cfm = cfm;
        this.logger = logger;
    }

    public string Name => this.GetType().Name;
    public Version Version => new Version(1, 0, 0);
    public string Description => "First migration to initialize the Schema";

    public async Task ApplyMigrationAsync()
    {
        this.logger.LogDebug($"Creating the Address User-Defined type...");
        await this.cfm.CreateUserDefinedTypeAsync<Address>();

        // Should not be here in real world application.
        // Used only for example purposes.
        this.cfm
            .GetCassandraSession()
                .UserDefinedTypes.Define(
                UdtMap.For<Address>()
                    .Map(a => a.Number, "Number".ToLower())
                    .Map(a => a.Street, "Street".ToLower())
                    .Map(a => a.City, "City".ToLower())
                    .Map(a => a.Country, "Country".ToLower())
                    .Map(a => a.Province, "Province".ToLower())
                    .Map(a => a.PostalCode, "PostalCode".ToLower()));

        this.logger.LogDebug($"Creating the User table...");
        await this.cfm.GetTable<Users>().CreateIfNotExistsAsync();
    }
}
```

* **Startup class**

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    // Custom method that you can create to initialize the Cassandra {ISession}.
    services.AddCassandraSession(this.Configuration);

    // Register the migrations
    services.AddTransient<IMigrator, InitialMigration>();

    // Required by the library to register the needed classes.
    services.AddCassandraFluentMigratorServices();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...

    // Start the migration process.
    app.UseCassandraMigration();

    ...
}
```
