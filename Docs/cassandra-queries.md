# Cassandra most used Queries

* Get the list of Keyspaces

```sql
DESCRIBE keyspaces;
```

* Start using a keyspace

```sql
use <keyspace_name>
```

* Get the list of tables

```sql
DESCRIBE TABLES
```

* Query a table

```sql
SELECT * FROM <table_name>
```

* Check if a table exists

```sql
SELECT table_name FROM system_schema.tables WHERE keyspace_name='<keyspace_name>' AND table_name='<table_name>';
```
