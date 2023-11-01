namespace Cassandra.Fluent.Migrator.Utils.Constants
{
    internal static class UdtCqlStatements
    {
        internal const string UDT_CREATE_STATEMENT = "CREATE TYPE IF NOT EXISTS {0} ( ";

        internal const string UDT_DROP_STATEMENT = "DROP TYPE IF EXISTS {0};";

        internal const string UDT_ADD_COLUMN_STATEMENT = "ALTER TYPE {0} ADD {1} {2};";

        internal const string UDT_RENAME_COLUMN_STATEMENT = "ALTER TYPE {0} RENAME {1} TO {2}";
    }
}