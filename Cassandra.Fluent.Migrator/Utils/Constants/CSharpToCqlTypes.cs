namespace Cassandra.Fluent.Migrator.Utils.Constants
{
    using System.Collections.Generic;
    using Cassandra.Fluent.Migrator.Utils.Extensions;

    internal static class CSharpToCqlTypes
    {
        /// <summary>
        /// Define the mapping between the CSharp and CQL types.
        /// </summary>
        internal static Dictionary<string, string> TypesMapping => new Dictionary<string, string>()
        {
            { "string", ColumnTypeCode.Text.NormalizeString() },
            { "guid", ColumnTypeCode.Uuid.NormalizeString() },
            { "timeuuid", ColumnTypeCode.Timeuuid.NormalizeString() },
            { "int", ColumnTypeCode.Int.NormalizeString() },
            { "int32", ColumnTypeCode.Int.NormalizeString() },
            { "byte", ColumnTypeCode.Blob.NormalizeString() },
            { "byte[]", ColumnTypeCode.Blob.NormalizeString() },
            { "float", ColumnTypeCode.Float.NormalizeString() },
            { "single", ColumnTypeCode.Float.NormalizeString() },
            { "double", ColumnTypeCode.Double.NormalizeString() },
            { "boolean", ColumnTypeCode.Boolean.NormalizeString() },
            { "bool", ColumnTypeCode.Boolean.NormalizeString() },
            { "ipaddress", ColumnTypeCode.Inet.NormalizeString() },
            { "localdate", ColumnTypeCode.Date.NormalizeString() },
            { "localtime", ColumnTypeCode.Time.NormalizeString() },
            { "short", ColumnTypeCode.SmallInt.NormalizeString() },
            { "int16", ColumnTypeCode.SmallInt.NormalizeString() },
            { "sbyte", ColumnTypeCode.TinyInt.NormalizeString() },
            { "datetime", ColumnTypeCode.Timestamp.NormalizeString() },
            { "long", ColumnTypeCode.Bigint.NormalizeString() },
            { "int64", ColumnTypeCode.Bigint.NormalizeString() },
            { "decimal", ColumnTypeCode.Decimal.NormalizeString() },
            { "biginteger", ColumnTypeCode.Varint.NormalizeString() },
            { "ienumerable", ColumnTypeCode.List.NormalizeString() },
            { "icollection", ColumnTypeCode.List.NormalizeString() },
            { "ilist", ColumnTypeCode.List.NormalizeString() },
            { "list", ColumnTypeCode.List.NormalizeString() },
            { "hashset", ColumnTypeCode.List.NormalizeString() },
            { "idictionary", ColumnTypeCode.Map.NormalizeString() },
        };
    }
}
