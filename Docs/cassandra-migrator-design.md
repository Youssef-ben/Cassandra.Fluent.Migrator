# Cassandra Fluent Migrator - Design document - Initial draft

This document describes the design and thoughts behind this library.

(P.S: The library will use the `Ilogger<TClass>` to log all important actions)

**_IMPORTANT: In order for the library to work, it need a valid Cassandra session and Keyspace_**

## Defining the needed Classes

This section defines the classes and their roles in the library.

### Cassandra Migrator

Interface : `ICassandraMigrator`

Handler   : `CassandraMigrator`

This class is the core of the library, it will fetch and execute the migrations.

In other words, The role of this Class is to execute all the registred Migrations from the Dependency Injection container
by fetching the list and iterate while checking that the migration isn't applied yet.

The Handler will expose the following methods:

```CSharp
/**
* Start the Migration process.
*
*  Should fetch the Migrators from the DI.
*  Should fetch the Applied migration from the DB and organize them by version.
*  Loop on the Migrators and check if we need to apply the migration.
*    - If yes, execute the Migrator method `ApplyMigration[Async]`.
*    - If not, skip with a message.
*  If the table [MigrationHistory] doesn't exists it should create it before.
*/
Migrate();
MigrateAsync();

/**
* Get the latest migration from the database
*/
GetLatestMigration();
GetLatestMigrationAsync();

/**
* Get the migrations from the the Dependency Injection `ServiceProvider`.
* Sort the list by version.
*/
GetMigrations();
GetMigrationsAsync();

/**
* Get The applied migration from the database.
* Sort the list by version.
*/
GetAppliedMigrations();
GetAppliedMigrationsAsync()
```

* This class will have its internal helper

```CSharp
/**
* Set the Cassandra Session and KeySpace.
* Will Create [MigrationsHistory] table if not exists.
* Note :
*     - Table Scheme : [MigrationHistory: {Name, Version, CreateAt, Description}
*/
InitFluentCassandraMigrator();
InitFluentCassandraMigratorAsync();

/**
* Validate that the Target version is a valid version.
* Checks if we should apply the migration.
*/
ShouldApplyMigration(IMigrator migration);
ShouldApplyMigrationAsync(IMigrator migration);

/**
* Save the Migration Data in the database.
*     - Create New Entry for the Migration in the {MigrationHistory}.
*/
UpdateMigrationHistory(SchemeDetails);
UpdateMigrationHistoryAsync(SchemeDetails);
```

### Migrator Interface

Name: `IMigrator`

This represents the interface to be used by the user migrations. all the migrations should implement this interface and use it to register the classes to the `Dependency Injection`.

```CSharp
/**
* Name of the current migration.
*/
string Name { get; set; }

/**
* Version of the current migration.
*/
Version Version { get; set; }

/**
* Description of the current migration (Value: Optional).
*/
string Description { get; set; }

/**
* This method will be used by the `CassandraMigrator` to execute the migration.
*/
ApplyMigration();
ApplyMigrationAsync();
```

### Migrator Helper

Interface: `ICassandraFluentMigrator`

Handler  : `CassandraFluentMigrator`

This Class will contain the methods that will be needed by the user to create his migrations. It should offer a set of methods that can be chained together for fluent code.

This helper will be devided to two sections (Class and extensions)

#### Class

This Class should be called in the constructor of the user migration and should be used as private property.

it define the following methods:

_**Note:** it's recomended to use strings instead of `nameof(...)` when using the migration methods this will allow to keep a certain consistency in your migrations._

_**Note:** the `[...]` represent optional parameter_

```CSharp
/// <summary>
/// Gets the registred Cassandra session context.
/// </summary>
/// <returns>Current instance of the Cassadnra session.</returns>
ISession GetCassandraSession();

/// <summary>
/// Gets the Cassandra table based on the {TEntity}.
/// </summary>
/// <typeparam name="TEntity">The Class that represent a table in Cassandra.</typeparam>
/// <returns>Instance of the Cassandra table.</returns>
Table<TEntity> GetTable<TEntity>()
    where TEntity : class;

/// <summary>
/// Checks if the table exists in the Cassandra session context.
/// </summary>
/// <param name="table">The table we need to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
/// <exception cref="NullReferenceException">Thrown when the table value is null or empty.</exception>
bool DoesTableExists([NotNull]string table);

/// <summary>
/// Checks if the column exits in the specified table.
/// </summary>
///
/// <remarks>
/// Before checking that the columns exists, the method first checks that the table exists.
/// </remarks>
///
/// <param name="table">The table we want to search.</param>
/// <param name="column">The column that we want to search for.</param>
/// <returns>True, if exists. False, Otherwise.</returns>
/// <exception cref="NullReferenceException">Thrown when the table or column value is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the table isn't available in the current casasndra session.</exception>
bool DoesColumnExists([NotNull]string table, [NotNull]string column);

/// <summary>
/// Checks if the User-Defined type exists in the Cassandra session context.
/// </summary>
/// <param name="udt">The User-Defined type that we need to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
/// <exception cref="NullReferenceException">Thrown when the User-Defined type value is null or empty.</exception>
bool DoesUdtExists([NotNull]string udt);

/// <summary>
/// Checks if the column exits in the specified User-Defined type.
/// </summary>
///
/// <remarks>
/// Before checking that the columns exists, the method first checks that the User-Defined type exists.
/// </remarks>
///
/// <param name="udt">The User-Defined type we want to search.</param>
/// <param name="column">The column that we want to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
/// <exception cref="NullReferenceException">Thrown when the User-Defined type or column value is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the User-Defined type isn't available in the current casasndra session.</exception>
bool DoesUdtColumnExists([NotNull]string udt, [NotNull]string column);

/// <summary>
/// Checks if the Materialized view exists in the Cassandra session context.
/// </summary>
/// <param name="view">The Materialized view that we need to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
/// <exception cref="NullReferenceException">Thrown when the Materialized view value is null or empty.</exception>
bool DoesMaterializedViewExists([NotNull]string view);

/// <summary>
/// Checks if the column exits in the specified Materialized view.
/// </summary>
///
/// <remarks>
/// Before checking that the columns exists, the method first checks that the Materialized view exists.
/// </remarks>
///
/// <param name="view">The Materialized view. we want to search.</param>
/// <param name="column">The column that we want to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
/// <exception cref="NullReferenceException">Thrown when the Materialized view or the column value is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the Materialized view isn't available in the current casasndra session.</exception>
bool DoesMaterializedViewColumnExists([NotNull]string view, [NotNull] string column);

/// <summary>
/// Get the Cassandra CQL type equivalent to the specified CSharp type.
/// </summary>
/// <param name="type">The CSharp type to be converted.</param>
/// <returns>Return string value containing the Cassandra CQL type.</returns>
string GetCqlType([NotNull]Type type);

/// <summary>
/// Get the Cassandra CQL type of the specified column.
/// </summary>
/// <typeparam name="TEntity">The class where we need to look for the column type.</typeparam>
/// <param name="column">The column that we want to search for.</param>
/// <returns>Return string value of the Cassandra CQL type.</returns>
string GetColumnType<TEntity>([NotNull]string column)
    where TEntity : class;
```

#### Extensions

* `Tables:` Set of methods to handle the [Creation/Alter/Rename/Delete] of a table columns.

```CSharp
/*
* Add a Column if not exists to the table. Otherwise it does nothing.
* Note :
*    - If the {Type: [Null || Empty]} the function will get the type from the {Entity} directly.
*/
AddColumnAsync("table", "field", ["Type"]);

/*
* Alter the type of the Column if exists. Otherwise it does nothing.
* Note :
*    - If the {Type: [Null || Empty]} the function will get the type from the {Entity} directly.
*/
AlterColumnAsync("table", "field", ["Type"]);

/*
* Rename the Column if exists. Otherwise it does nothing.
*/
RenameColumnAsync("table", "old", "new");

/*
* Delete the Column if exists. Otherwise it does nothing.
*/
DeleteColumnAsync("table", "field");
```

* `User Defined Types:` Sets of methods to handle the [Creation/Alter/Rename/Delete] of a User-Defined Type.

```CSharp
/*
* Create a new User-Defined Type if not exists. Otherwise it does nothing.
* Note :
*    - If the [UdtName: {Null || Empty}] the function will take the Entity name.
*/
CreateUserDefinedTypeAsync("UdtName");

/*
* Drop a new User-Defined Type if not exists. Otherwise it does nothing.
* Note :
*    - If the [UdtName: {Null || Empty}] the function will take the Entity name.
*/
DropUserDefinedTypeAsync("UdtName");

// ***************** [Add/Alter/Rename/Delete] Column from a User-Defined Type ***************** //

/*
* Add Column to Udt if not Exists. Otherwise it does nothing.
* Note :
*    - If the [Type: {Null || Empty}] the function will take the Type from the Entity.
*/
AlterUdtAddColumnAsync("udt", "field", ["Type"]);

/*
* Alter Udt Column if Exists. Otherwise it does nothing.
* Note :
*    - If the [Type: {Null || Empty}] the function will take the Type from the Entity.
*/
AlterUdtAlterColumnAsync("udt", "field", ["Type"]);

/*
* Rename Udt Column if Exists. Otherwise it does nothing.
*/
AlterUdtRenameColumnAsync("udt", "old", "new");

/*
* Delete Udt Column if Exists. Otherwise it does nothing.
*/
AlterUdtDeleteColumnAsync("udt", "field");
```

* `Materialized View:` Set of methods to handle the [Creation/Alter/Rename/Delete] of a Materialized View.

_`Note:` it's prefered to delete and create the view with the specief values columns and configuration_

```CSharp
/*
* Create a new Materialized View if not exists. Otherwise it does nothing.
* Note :
*    - PrimaryKeyFieldName : The field that will be a primary key for the View. (Required)
*    - SecondaryKeyFieldName: The field that will be a secondary key. Will be ignored if empty. (Optional)
*    - ClusterKeyFieldName: The field that will be a cluster key. Will be ignored if empty. (Optional)
*    - ViewName : If the field is {Null || Empty} it will take the Entity name.
*/
CreateViewAsync("ViewName", "PrimaryKeyFieldName", "SecondaryKeyFieldName", "ClusterKeyFieldName");
CreateViewAsync("ViewName", "PrimaryKeyFieldName", "SecondaryKeyFieldName");
CreateViewAsync("ViewName", "PrimaryKeyFieldName");
CreateViewAsync("PrimaryKeyFieldName", "SecondaryKeyFieldName", "ClusterKeyFieldName");
CreateViewAsync("PrimaryKeyFieldName", "SecondaryKeyFieldName");
CreateViewAsync("PrimaryKeyFieldName");

/*
* Delete the Materialized View if exists. Otherwise it does nothing.
* Note :
*    - If the {ViewName: [Null || Empty]} the function will take the Entity name.
*/
DropViewAsync("ViewName");
DropViewAsync();
```
