namespace Cassandra.Fluent.Migrator.Utils.Constants
{
    internal static class AppErrorsMessages
    {
        internal const string COLUMN_NOT_FOUND = "Column {0} was not found in table/User-defined type [{1}]!";

        internal const string COLUMN_EXISTS = "Invalid column name [{0}], because it conflicts with an existing column!";

        internal const string NULL_ARGUMENT = "The parameter should not be empty!";

        internal const string NULL_REFERENCE = "The field [{0}] was not found in the specified type [{1}]. Add the field in the object or check the field spelling!";

        internal const string TYPE_COLUMN_EXISTS = "A field of the same name already exists!";

        internal const string TYPE_UDT_COLUMN_EXISTS = "Cannot add new field [{0}] to type!";

        internal const string TYPE_COLUMN_NOT_FOUND = "Unknown field [{0}] in type!";

        internal const string TYPE_NOT_SUPPORTED = "The Type [{0}] is not supported!";

        internal const string CAN_NOT_RENAME_NONE_PRIMARY_KEY = "The [{0}] is not a primary key. You can only rename primary keys!";

        internal const string COLUMN_EXISTS_FOR_RENAME = "Cannot rename column {0} to {1} in keyspace {2}, another column with the same name already exist!";

        internal const string OBJECT_NOT_FOUND = "The {0} [{1}], not found in the specified Cassandra keyspace [{2}]!";
    }
}
