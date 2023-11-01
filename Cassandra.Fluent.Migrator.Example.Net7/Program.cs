using Cassandra.Fluent.Migrator;
using Cassandra.Fluent.Migrator.Core;
using Cassandra.Fluent.Migrator.Example.Net7.Extensions;
using Cassandra.Fluent.Migrator.Example.Net7.Migrations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
        .AddCassandraSession(builder.Configuration)
        .AddCassandraMigrations();

// Inserted from the Cassandra.Fluent.Migrator
builder.Services.AddCassandraFluentMigratorServices();

WebApplication app = builder.Build();

const string BASE_URI = "/api/migrator";
const string MIGRATION_LIST = $"{BASE_URI}/migrations";
const string MIGRATION_APPLIED_LIST = $"{BASE_URI}/applied";
const string MIGRATION_LATEST_LIST = $"{BASE_URI}/latest";
IList<string> availableRoutes = new List<string> { MIGRATION_LIST, MIGRATION_APPLIED_LIST, MIGRATION_LATEST_LIST };

app.MapGet("/",
        () => Results.Ok(new { availableRoutes }));
app.MapGet(MIGRATION_LIST,
        (ICassandraMigrator migrator) => Results.Ok(migrator.GetRegisteredMigrations()));
app.MapGet(MIGRATION_APPLIED_LIST,
        (ICassandraMigrator migrator) => Results.Ok(migrator.GetAppliedMigrations()));
app.MapGet(MIGRATION_LATEST_LIST,
        (ICassandraMigrator migrator) => Results.Ok(migrator.GetLatestMigration()));

// Inserted from the Cassandra.Fluent.Migrator
app.UseCassandraMigration();

app.Run();