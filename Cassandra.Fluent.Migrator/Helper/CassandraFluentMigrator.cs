namespace Cassandra.Fluent.Migrator.Helper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Data.Linq;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;
    using Utils.Constants;
    using Utils.Exceptions;
    using Utils.Extensions;

    public class CassandraFluentMigrator : ICassandraFluentMigrator
    {
        private readonly string cassandraKeyspace;
        private readonly ISession cassandraSession;

        public CassandraFluentMigrator(ISession session)
        {
            cassandraSession = session;
            cassandraKeyspace = cassandraSession.Keyspace;
        }

        public ISession GetCassandraSession()
        {
            return cassandraSession;
        }

        public Table<TEntity> GetTable<TEntity>()
                where TEntity : class
        {
            return new Table<TEntity>(cassandraSession);
        }

        public bool DoesTableExists([NotNull] string table)
        {
            Check.NotEmptyNotNull(table, $"The argument [{nameof(table)}]");

            return cassandraSession
                    .Cluster
                    .Metadata
                    .GetTables(cassandraKeyspace)
                    .Any(x => x.NormalizeString() == table.NormalizeString());
        }

        public bool DoesColumnExists([NotNull] string table, [NotNull] string column)
        {
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            if (DoesTableExists(table))
            {
                return cassandraSession
                        .Cluster
                        .Metadata
                        .GetTable(cassandraKeyspace, table.NormalizeString())
                        .TableColumns
                        .AsEnumerable()
                        .Any(x => x.Name.NormalizeString() == column.NormalizeString());
            }

            var errorMsg = AppErrorsMessages.OBJECT_NOT_FOUND.NormalizeString("table", table, cassandraKeyspace);
            throw new ObjectNotFoundException(errorMsg);
        }

        public bool DoesUdtExists([NotNull] string udt)
        {
            Check.NotEmptyNotNull(udt, $"The argument [{nameof(udt)}]");

            UdtColumnInfo result = cassandraSession
                    .Cluster
                    .Metadata
                    .GetUdtDefinition(cassandraKeyspace, udt.NormalizeString());

            return result != null;
        }

        public bool DoesUdtColumnExists([NotNull] string udt, [NotNull] string column)
        {
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            if (DoesUdtExists(udt))
            {
                return cassandraSession
                        .Cluster
                        .Metadata
                        .GetUdtDefinition(cassandraKeyspace, udt.NormalizeString())
                        .Fields
                        .Exists(x => x.Name.NormalizeString() == column.NormalizeString());
            }

            var errorMsg = AppErrorsMessages.OBJECT_NOT_FOUND
                    .NormalizeString("User-Defined type", udt, cassandraKeyspace);
            throw new ObjectNotFoundException(errorMsg);
        }

        public bool DoesMaterializedViewExists([NotNull] string view)
        {
            Check.NotEmptyNotNull(view, $"The argument [{nameof(view)}]");

            MaterializedViewMetadata result = cassandraSession
                    .Cluster
                    .Metadata
                    .GetMaterializedView(cassandraKeyspace, view);

            return result != null;
        }

        public bool DoesMaterializedViewColumnExists([NotNull] string view, [NotNull] string column)
        {
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            if (DoesMaterializedViewExists(view))
            {
                return cassandraSession
                        .Cluster
                        .Metadata
                        .GetMaterializedView(cassandraKeyspace, view)
                        .ColumnsByName
                        .Any(x => x.Key.NormalizeString() == column.NormalizeString());
            }

            var errorMsg = AppErrorsMessages.OBJECT_NOT_FOUND
                    .NormalizeString("Materialized view", view, cassandraKeyspace);
            throw new ObjectNotFoundException(errorMsg);
        }

        public string GetCqlType([NotNull] Type type, bool shouldBeFrozen = false)
        {
            Check.NotNull(type, $"The argument [{nameof(type)}]");

            return type.GetCqlType(shouldBeFrozen);
        }

        public string GetColumnType<TEntity>([NotNull] string column, bool shouldBeFrozen = false)
                where TEntity : class
        {
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            return typeof(TEntity).GetCqlType(column, shouldBeFrozen);
        }
    }
}