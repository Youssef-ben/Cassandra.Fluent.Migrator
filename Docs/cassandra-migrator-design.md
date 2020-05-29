# Cassandra Fluent Migrator - Design document

This document describes the design and thoughts behind this library (Should be deleted)

(P.S: The library will use the `Ilogger<TClass>` to log all important actions)

**_IMPORTANT: In order for the library to work, it need a valid Cassandra session and Keyspace_**

## Defining the needed Classes

This section defines the classes and their roles in the library.

### Cassandra Migrator

Interface : `ICassandraMigrator`

Handler   : `CassandraMigrator`

This class is the core of the library, most of the logic to fetch and execute the migration will be handled by it.

In other words, The role of this Class is to execute all the registred Migration from the Dependency Injection
by fetching the list and iterate while checking that it's not applied yet.

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
*/
MigrateAsync();

/**
* Get the latest migration from the database
*/
GetLatestMigration();

/**
* Get the latest version from the database.
*/
GetLatestVesion();

/**
* Get the migrations from the the Dependency Injection `ServiceProvider`.
* Sort the list by version.
*/
GetMigrations()

/**
* Get The applied migration from the database.
* Sort the list by version.
*/
GetAppliedMigrations()
```

* This class wil have its internal helper

```CSharp
/**
* Set the Cassandra Session and KeySpace.
* Will Create [MigrationsHistory] table if not exists.
* Note :
*     - Table Scheme : [MigrationHistory: {Name, Version, CreateAt, Description}
*/
InitFluentCassandraMigrator();

/**
* Validate that the Target version is a valid version.
* Checks if we should apply the migration.
*/
ShouldApplyMigrationAsync(IMigrator migration);

/**
* Save the Migration Data in the database.
*     - Create New Entry for the Migration in the {MigrationHistory}.
*/
UpdateMigrationHistory(SchemeDetails);
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

### Migration Helper

Interface: `IFluentCassandraMigrator`

Handler  : `FluentCassandraMigrator`

This Class will contain the methods that will be needed by the user to create his migrations. It offers a set of method that can be chained together for fluent code.

This helper will be devided to two sections (Class and extensions)

#### Class

This Class should be called in the constructor of the user migration and should be used as private property.
it define the following methods:

_**Note:** it's recomended to use strings instead of `nameof(...)` when using the migration methods this will allow to keep a certain consistency in your migrations._

_**Note:** the `[...]` represent optional parameter_

```CSharp
/*
* Set the Cassandra Session Context.
*/
GetCassandraSession();

/*
* Get a Cassandra Instance of the Table.
*/
GetTable("TableName");

/*
* Checks if the Table exists in the Database.
*/
DoesTableExists("TableName");

/*
* Checks if the column exists in the table or User-Defined Type.
*/
DoesColumnExists("TableName", "ColumnName")

/*
* Checks if the Materialized View Exist in the Database.
*/
DoesViewExits("ViewName");

/*
* Checks if the User-Defined Type exists in the Database.
*/
DoesUdtExists("UdtName");

/*
* Checks if the User-Defined Type has the Specified Column.
*/
DoesUdtColumnExists("UdtName", "ColumnName");

/*
* Convert C# Type to Cassandra Type (CQL or UDT)
*
* tryAction: The Try Action that should be executed to convert the CSharp type.
*   - SYSTME_TYPE
*   - LIST_TYPES
*   - USER_DEFINED_TYPES
*/
ConvertToCQLType(Type, TryAction);

/*
* Convert the Current Type to a CQL List.
*/
ConvertToCqlList(Type);

/*
* Convert the Current Type to a CQL User-Defined Type.
*/
ConvertToCqlUdt(Type);

/*
* Get the Type of the Field from the Caller Object.
*/
GetFieldType("tableName", "FieldName");
```

#### Extensions

* `Tables:` Set of methods to handle the [Creation/Alter/Rename/Delete] of a table columns.

```CSharp

CreateTableAsync("table");

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
