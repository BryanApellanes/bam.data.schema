using Bam.Data.Schema;

namespace Bam.Data.FirebirdSql
{
    /// <summary>
    /// Extracts schema definitions from a Firebird SQL database. Currently not implemented.
    /// </summary>
    public class FirebirdSqlSchemaExtractor : DaoSchemaExtractor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FirebirdSqlSchemaExtractor"/> for the specified database.
        /// </summary>
        /// <param name="database">The Firebird SQL database to extract schema from.</param>
        public FirebirdSqlSchemaExtractor(FirebirdSqlDatabase database)
        {
            Database = database;
            ConnectionString = database.ConnectionString!;
        }
        /// <inheritdoc />
        public override DataTypes GetColumnDataType(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetColumnDbDataType(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetColumnMaxLength(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string[] GetColumnNames(string tableName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool GetColumnNullable(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ForeignKeyColumn[] GetForeignKeyColumns()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetKeyColumnName(string tableName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string GetSchemaName()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string[] GetTableNames()
        {
            throw new NotImplementedException();
        }

        protected override void SetConnectionName(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
