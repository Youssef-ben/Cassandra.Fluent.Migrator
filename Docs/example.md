# Example (Should be Deleted and added in a getting started doc)

```CSharp
// Should Check for Schema Agreement before going to next statement
public class AddFieldsToTableXXXX: IMigrator{
    private FluentCassandraMigrator migrator;

    public AddFieldsToTableXXXX(FluentCassandraMigrator migrator){
        this.migrator = migrator;
    }

    public string Name => This.GetType().Name;
    public Version Version => new Version ("1.0.0");
    public string description => "Migration to add fields to table xxxx";

    public void ApplyMigrationAsync(){
        // Migration for Tables
        this.migrator.AddColumnToTableAsync(table, field, type);
        this.migrator.AddColumnToTableAsync(table, field);

        this.migrator.AddColumnToTableAsync<Table>(field, type);
        this.migrator.AddColumnToTableAsync<Table>(field);

        // Migration for UDTs
        this.migrator.AddColumnToUdtAsync(udt, field, type);
        this.migrator.AddColumnToUdtAsync(udt, field);

        this.migrator.AddColumnToUdtAsync<Udt>(field, type);
        this.migrator.AddColumnToUdtAsync<Udt>(field);
    }
}
```


```CSharp
public void ConfigureServices(IServiceCollection services)
{
	...
	// Should Configure the Cassandra Session before.
	...
	seviceProvider
		.AddSingleton<IMigrator, AddFieldsToTableXXXX>();
	...
}

public void Configure(IApplicationBuilder app)
{
	...
	// Should be declared at the end.
	// Will run the `CassandraMigrator.Migrate()`
	app.UseCassandraMigration();
}
...
```

