# Cassandra Fluent Migrator - Design document - Initial draft

This document describes the design and thoughts behind this library.

(P.S: The library will use the `Ilogger<TClass>` to log all important actions)

**_IMPORTANT: In order for the library to work, it need a valid Cassandra session and Keyspace_**

## Defining the needed Classes

This section defines the classes and their roles in the library.

### Cassandra Migrator

Interface : `ICassandraMigrator`

Handler   : `CassandraMigrator`

This class is the core of the library, it fetchs and execute the migrations.

In other words, The role of this Class is to execute all the registred Migrations from the Dependency Injection container
by fetching the list and iterate while checking that the migration isn't applied yet.

The Handler will expose the following methods:

```CSharp
/// <summary>
/// Start the migration process.
///
/// The method fetch the registred migrations from the {Services Provider} of the app.
/// Before appling a migration, the method checks if its already applied, If True, it skipps
/// the migration otherwise applies it using the {ApplyMigration()} of the Migration.
/// </summary>
///
/// <returns>Count of the applied migrations.</returns>
internal int Migrate();

/// <summary>
/// Get the latest migration that was applied to the schema.
/// </summary>
///
/// <returns>Migration history details.</returns>
MigrationHistory GetLatestMigration();

/// <summary>
/// Gets the list of the registred migrations from the app {services provider}.
/// The migrations are automatically sorted older to latest.
/// </summary>
///
/// <returns>List of migrations.</returns>
ICollection<IMigrator> GetRegistredMigrations();

/// <summary>
/// Gets the list of the applied migrations from the databse.
/// The migrations are automatically sorted latest to older.
/// </summary>
///
/// <returns>List of migrations.</returns>
ICollection<MigrationHistory> GetAppliedMigration();
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
///
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
///
/// <exception cref="NullReferenceException">Thrown when the table or column value is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the table isn't available in the current casasndra session.</exception>
bool DoesColumnExists([NotNull]string table, [NotNull]string column);

/// <summary>
/// Checks if the User-Defined type exists in the Cassandra session context.
/// </summary>
/// <param name="udt">The User-Defined type that we need to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
///
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
///
/// <exception cref="NullReferenceException">Thrown when the User-Defined type or column value is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the User-Defined type isn't available in the current casasndra session.</exception>
bool DoesUdtColumnExists([NotNull]string udt, [NotNull]string column);

/// <summary>
/// Checks if the Materialized view exists in the Cassandra session context.
/// </summary>
/// <param name="view">The Materialized view that we need to search for.</param>
/// <returns>True, if exists. False otherwise.</returns>
///
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
///
/// <exception cref="NullReferenceException">Thrown when the Materialized view or the column value is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the Materialized view isn't available in the current casasndra session.</exception>
bool DoesMaterializedViewColumnExists([NotNull]string view, [NotNull] string column);

/// <summary>
/// Get the Cassandra CQL type equivalent to the specified CSharp type.
/// </summary>
/// <param name="type">The CSharp type to be converted.</param>
/// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
/// <returns>Return string value containing the Cassandra CQL type.</returns>
string GetCqlType([NotNull]Type type, bool shouldBeFrozen = false);

/// <summary>
/// Get the Cassandra CQL type of the specified column.
/// </summary>
/// <typeparam name="TEntity">The class where we need to look for the column type.</typeparam>
/// <param name="column">The column that we want to search for.</param>
/// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
/// <returns>Return string value of the Cassandra CQL type.</returns>
string GetColumnType<TEntity>([NotNull]string column, bool shouldBeFrozen = false)
    where TEntity : class;
```

#### Extensions

* `Tables:` Set of methods to handle the [Creation/Alter/Rename/Delete] of a table columns.

```CSharp
/// <summary>
/// Adds the specified column to the targeted table only if the column doesn't exists.
/// </summary>
///
/// <param name="self">The instance of the Cassandra Fluent Migratr helper.</param>
/// <param name="table">The table to which we want to add the new column.</param>
/// <param name="column">The new column.</param>
/// <param name="type">The new column type.</param>
/// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
/// <returns>The instance of the Cassandra Fluent Migrator helper.</returns>
///
/// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
Task<ICassandraFluentMigrator> AddColumnAsync(string table, string column, Type type, bool shouldBeFrozen = false);
Task<ICassandraFluentMigrator> AddColumnAsync<TTableClass>(string table, string column, bool shouldBeFrozen = false);

/// <summary>
/// Rename the specified column in the targeted table only if the column exists.
/// <para>IMPORTANT: In Cassandra only the Primary key can be renamed.</para>
/// </summary>
///
/// <typeparam name="Table">The Table where we should look for the column type.</typeparam>
/// <param name="self">Cassandra Table.</param>
/// <param name="table">The table.</param>
/// <param name="old">The column to be renamed.</param>
/// <param name="target">The new column name.</param>
/// <returns>The table Instance.</returns>
///
/// <exception cref="ArgumentNullException">Thrown when one of the arguments is null or empty.</exception>
/// <exception cref="ApplicationException">Thrown when the Column is not a primary key.</exception>
Task<ICassandraFluentMigrator> RenamePrimaryColumnAsync(this ICassandraFluentMigrator self, string table, string old, string target);

/// <summary>
/// Drops the specified column from the targeted table only if the column exists.
/// </summary>
///
/// <param name="self">The instance of the Cassandra Fluent Migrator helper.</param>
/// <param name="table">The table from which we want to delete the column.</param>
/// <param name="column">The column to be deleted.</param>
/// <returns>The table Instance.</returns>
///
/// <exception cref="ArgumentNullException">Thrown when one of the arguments is null or empty.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the table doesn't exists.</exception>
Task<ICassandraFluentMigrator> DropColumnAsync(this ICassandraFluentMigrator self, [NotNull]string table, [NotNull]string column);


// IMPORTANT: Alter Column is no longer supported in Cassandra v3.x
Task<ICassandraFluentMigrator> AlterColumnAsync("table", "field", ["Type"]);
```

* `User Defined Types:` Sets of methods to handle the [Creation/Alter/Rename/Delete] of a User-Defined Type.

```CSharp
/// <summary>
/// Create the new User-Defined type by building and generating a query
/// based on the generic class fields and types.
/// If the UDT already exists the method skips the creation.
///
/// <para>Note: If the udt name is [Null || Empty] the method will take the generic type {TEntity} name.</para>
/// </summary>
///
/// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
/// <param name="self">The Cassandra Fluent Migrator.</param>
/// <param name="name">The name of the udt (Optional).</param>
/// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
/// <returns>The Cassandra CQL query.</returns>
///
/// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
Task<ICassandraFluentMigrator> CreateUserDefinedTypeAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, bool shouldBeFrozen = false);


/// <summary>
/// Delete the User-Defined type if exists.
/// If the UDT doesn't exists the method skips the creation.
///
/// <para>Note: If the udt name is [Null || Empty] the method will take the generic type {TEntity} name.</para>
/// </summary>
///
/// <typeparam name="TEntity">The calss where the method should look for the properties and their types.</typeparam>
/// <param name="self">The Cassandra Fluent Migrator.</param>
/// <param name="name">The name of the udt (Optional).</param>
/// <returns>The Cassandra CQL query.</returns>
///
/// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
Task<ICassandraFluentMigrator> DeleteUserDefinedTypeAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull]string name = default);


// ***************** [Add/Alter/Rename/Delete] Column from a User-Defined Type ***************** //

/// <summary>
/// Alter the specified Uder-Defined type by adding a new column only if it doesn't exists.
/// If the UDT exists the method skips the creation.
/// </summary>
///
/// <param name="self">The Cassandra Fluent Migrator.</param>
/// <param name="udt">The name of the User-Defined type.</param>
/// <param name="column">The name of the column to be added.</param>
/// <param name="type">The type of the new column.</param>
/// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
/// <returns>The Cassandra CQL query.</returns>
///
/// <exception cref="NullReferenceException">Thrown when the arument are invalid or the specified type doesn't exists.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the udt doesn't exists.</exception>
Task<ICassandraFluentMigrator> AlterUdtAddColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, string column, Type type, bool shouldBeFrozen = false);
Task<ICassandraFluentMigrator> AlterUdtAddColumnAsync<TEntity>([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, string column, bool shouldBeFrozen = false);

/// <summary>
/// Alter the specified User-Defined type by renaming the column name by the target name.
/// In case the target name exists the method throws an exception.
/// </summary>
///
/// <param name="self">The Cassandra Fluent Migrator.</param>
/// <param name="udt">The name of the User-Defined type.</param>
/// <param name="column">The name of the column to be renamed.</param>
/// <param name="target">The new column name.</param>
/// <returns>The Cassandra Fluent Migrator helper.</returns>
///
/// <exception cref="ArgumentNullException">Thrown when one of the arguments is null or empty.</exception>
/// <exception cref="InvalidOperationException">Thrown when the target column name exists.</exception>
/// <exception cref="ObjectNotFoundException">Thrown when the udt doesn't exists.</exception>
Task<ICassandraFluentMigrator> AlterUdtRenameColumnAsync([NotNull]this ICassandraFluentMigrator self, [NotNull]string udt, [NotNull]string column, [NotNull]string target);


// IMPORTANT: Cassandra doesn't support Dropping a column of a type.
AlterUdtDeleteColumnAsync("udt", "field");

// IMPORTANT: Alter Column is no longer supported in Cassandra v3.x
AlterUdtAlterColumnAsync("udt", "field", ["Type"]);
```
