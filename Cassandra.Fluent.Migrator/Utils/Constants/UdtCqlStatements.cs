namespace Cassandra.Fluent.Migrator.Utils.Constants
{
    internal class UdtCqlStatements
    {
        internal const string TYPE_CREATE_STATEMENT = "CREATE TYPE IF NOT EXISTS {0} ( ";

        internal const string TYPE_DROP_STATEMENT = "DROP TYPE IF EXISTS {0};";

        internal const string TYPE_ADD_COLUMN_STATEMENT = "ALTER TYPE {0} ADD {1} {2};";

        internal const string TYPE_RENAME_COLUMN_STATEMENT = "ALTER TYPE {0} RENAME {1} TO {2}";
    }
}
