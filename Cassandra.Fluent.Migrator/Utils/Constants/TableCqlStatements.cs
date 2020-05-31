namespace Cassandra.Fluent.Migrator.Utils.Constants
{
    internal static class TableCqlStatements
    {
        internal const string TABLE_ADD_COLUMN_STATEMENT = "ALTER TABLE {0} ADD {1} {2};";

        internal const string TABLE_ALTER_COLUMN_STATEMENT = "ALTER TABLE {0} ALTER {1} TYPE {2};";

        internal const string TABLE_RENAME_COLUMN_STATEMENT = "ALTER TABLE {0} RENAME {1} TO {2}";

        internal const string TABLE_DROP_COLUMN_STATEMENT = "ALTER TABLE {0} DROP {1};";
    }
}
