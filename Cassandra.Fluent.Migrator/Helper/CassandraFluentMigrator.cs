namespace Cassandra.Fluent.Migrator.Helper
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Cassandra;
    using Cassandra.Data.Linq;
    using Cassandra.Fluent.Migrator.Utils.Constants;
    using Cassandra.Fluent.Migrator.Utils.Exceptions;
    using Cassandra.Fluent.Migrator.Utils.Extensions;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    public class CassandraFluentMigrator : ICassandraFluentMigrator
    {
        private readonly ISession cassandraSession;
        private readonly string cassandraKeyspace;

        public CassandraFluentMigrator(ISession cassandraSession)
        {
            this.cassandraSession = cassandraSession;
            this.cassandraKeyspace = this.cassandraSession.Keyspace;
        }

        public ISession GetCassandraSession()
        {
            return this.cassandraSession;
        }

        public Table<TEntity> GetTable<TEntity>()
            where TEntity : class
        {
            return new Table<TEntity>(this.cassandraSession);
        }

        public bool DoesTableExists([NotNull]string table)
        {
            Check.NotEmptyNotNull(table);

            return this.cassandraSession
                .Cluster
                .Metadata
                .GetTables(this.cassandraKeyspace)
                .Any(x => x.NormalizeString() == table.NormalizeString());
        }

        public bool DoesColumnExists([NotNull]string table, [NotNull]string column)
        {
            Check.NotEmptyNotNull(column);

            if (!this.DoesTableExists(table))
            {
                var errorMsg = AppErrorsMessages.OBJECT_NOT_FOUND.NormalizeString("table", table, this.cassandraKeyspace);
                throw new ObjectNotFoundException(errorMsg);
            }

            return this.cassandraSession
                .Cluster
                .Metadata
                .GetTable(this.cassandraKeyspace, table.NormalizeString())
                .TableColumns
                .Any(x => x.Name == column.NormalizeString());
        }

        public bool DoesUdtExists([NotNull]string udt)
        {
            Check.NotEmptyNotNull(udt);

            var result = this.cassandraSession
                .Cluster
                .Metadata
                .GetUdtDefinition(this.cassandraKeyspace, udt);

            return result != null;
        }

        public bool DoesUdtColumnExists([NotNull]string udt, [NotNull]string column)
        {
            Check.NotEmptyNotNull(column);

            if (!this.DoesUdtExists(udt))
            {
                var errorMsg = AppErrorsMessages.OBJECT_NOT_FOUND.NormalizeString("User-Defined type", udt, this.cassandraKeyspace);
                throw new ObjectNotFoundException(errorMsg);
            }

            return this.cassandraSession
                .Cluster
                .Metadata
                .GetUdtDefinition(this.cassandraKeyspace, udt)
                .Fields
                .Any(x => x.Name.NormalizeString() == column.NormalizeString());
        }

        public bool DoesMaterializedViewExists([NotNull]string view)
        {
            Check.NotEmptyNotNull(view);

            var result = this.cassandraSession
                .Cluster
                .Metadata
                .GetMaterializedView(this.cassandraKeyspace, view);

            return result != null;
        }

        public bool DoesMaterializedViewColumnExists([NotNull]string view, [NotNull] string column)
        {
            Check.NotEmptyNotNull(column);

            if (!this.DoesMaterializedViewExists(view))
            {
                var errorMsg = AppErrorsMessages.OBJECT_NOT_FOUND.NormalizeString("Materialized view", view, this.cassandraKeyspace);
                throw new ObjectNotFoundException(errorMsg);
            }

            return this.cassandraSession
                .Cluster
                .Metadata
                .GetMaterializedView(this.cassandraKeyspace, view)
                .ColumnsByName
                .Any(x => x.Key.NormalizeString() == column.NormalizeString());
        }

        public string GetCqlType([NotNull]Type type)
        {
            Check.NotNull(type);

            return type.GetCqlType();
        }

        public string GetColumnType<TEntity>([NotNull]string column)
            where TEntity : class
        {
            Check.NotEmptyNotNull(column);

            return typeof(TEntity).GetCqlType();
        }
    }
}
