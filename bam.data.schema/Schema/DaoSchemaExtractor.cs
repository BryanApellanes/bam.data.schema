/*
	Copyright © Bryan Apellanes 2015  
*/

using Bam.Logging;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Abstract base class for extracting database schema definitions from a live database, converting table and column metadata into a <see cref="DaoSchemaDefinition"/>.
    /// </summary>
    public abstract class DaoSchemaExtractor : Loggable, IDaoSchemaExtractor, IHasSchemaTempPathProvider
    {
        readonly Dictionary<DaoSchemaExtractorNamingCollisionStrategy, Func<string, string, string, string>> _namingCollisionHandlers = new Dictionary<DaoSchemaExtractorNamingCollisionStrategy, Func<string, string, string, string>>();
        public DaoSchemaExtractor()
        {
            NameMap = new SchemaNameMap();
            NameFormatter = new SchemaNameMapNameFormatter(NameMap);
            SchemaTempPathProvider = sd => RuntimeSettings.ProcessDataFolder;
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.LeadingUnderscore, (tableName, columnName, propertyName) => $"_{columnName}");
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.TrailingUnderscore, (tableName, columnName, propertyName) => $"{columnName}_");
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.TypePrefix, (tableName, columnName, propertyName) => $"{GetColumnDataType(tableName, columnName)}{columnName}");
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.TypeSuffix, (tableName, columnName, propertyName) => $"{columnName}{GetColumnDataType(tableName, columnName)}");
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.UnderscoreDelimit, (tableName, columnName, propertyName) => $"_{columnName}_");
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.Custom, (tableName, columnName, propertyName) => CustomNamingCollisionHandler(tableName, columnName, propertyName));
            _namingCollisionHandlers.Add(DaoSchemaExtractorNamingCollisionStrategy.Invalid, (tableName, columnName, propertyName) => throw new InvalidOperationException("Invalid SchemaExtractorNamingCollisionStrategy specified"));
            CustomNamingCollisionHandler = _namingCollisionHandlers[DaoSchemaExtractorNamingCollisionStrategy.TrailingUnderscore];
            SchemaExtractorNamingCollisionStrategy = DaoSchemaExtractorNamingCollisionStrategy.TrailingUnderscore;
        }

        /// <summary>
        /// Gets the database this extractor reads schema metadata from.
        /// </summary>
        public Database Database { get; protected set; }

        /// <summary>
        /// Occurs when a table begins processing.
        /// </summary>
        public event EventHandler ProcessingTable;
        /// <summary>
        /// Occurs when a table finishes processing.
        /// </summary>
        public event EventHandler ProcessingTableComplete;

        /// <summary>
        /// Occurs when a column begins processing.
        /// </summary>
        public event EventHandler ProcessingColumn;

        /// <summary>
        /// Occurs when a column finishes processing.
        /// </summary>
        public event EventHandler ProcessingColumnComplete;

        /// <summary>
        /// Occurs when a foreign key begins processing.
        /// </summary>
        public event EventHandler ProcessingForeignKey;

        /// <summary>
        /// Occurs when a foreign key finishes processing.
        /// </summary>
        public event EventHandler ProcessingForeignComplete;

        /// <summary>
        /// Occurs before a class name is formatted from a table name.
        /// </summary>
        public event EventHandler ClassNameFormatting;

        /// <summary>
        /// Occurs after a class name has been formatted from a table name.
        /// </summary>
        public event EventHandler ClassNameFormatted;

        /// <summary>
        /// Occurs before a property name is formatted from a column name.
        /// </summary>
        public event EventHandler PropertyNameFormatting;

        /// <summary>
        /// Occurs after a property name has been formatted from a column name.
        /// </summary>
        public event EventHandler PropertyNameFormatted;

        /// <summary>
        /// Occurs when a property name collision with a reserved keyword or containing type is avoided.
        /// </summary>
        public event EventHandler PropertyNameCollisionAvoided;

        public abstract string GetSchemaName();
        public abstract string[] GetTableNames();
        public abstract string GetKeyColumnName(string tableName);
        public abstract string[] GetColumnNames(string tableName);
        public abstract DataTypes GetColumnDataType(string tableName, string columnName);
        public abstract string GetColumnDbDataType(string tableName, string columnName);
        public abstract string GetColumnMaxLength(string tableName, string columnName);
        public abstract bool GetColumnNullable(string tableName, string columnName);
        public abstract ForeignKeyColumn[] GetForeignKeyColumns();
        protected abstract void SetConnectionName(string connectionString);

        string _connectionString;
        public virtual string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                SetConnectionName(value);
            }
        }

        /// <summary>
        /// A function used to avoid naming collisions on property names and reserved
        /// Dao keywords.  Receives tableName, columnName and propertyName; should return
        /// desired propertyName
        /// </summary>
        public Func<string, string, string, string> CustomNamingCollisionHandler { get; set; }
        /// <summary>
        /// Gets or sets the strategy for resolving property name collisions with reserved keywords.
        /// </summary>
        public DaoSchemaExtractorNamingCollisionStrategy SchemaExtractorNamingCollisionStrategy { get; set; }

        /// <summary>
        /// Gets or sets the mapping between database names and C# class/property names.
        /// </summary>
        public SchemaNameMap NameMap { get; set; }

        /// <summary>
        /// Gets or sets the formatter used for deriving class and property names from table and column names.
        /// </summary>
        public INameFormatter NameFormatter { get; set; }

        /// <summary>
        /// Gets or sets a function that provides a temporary path for schema output based on the schema definition.
        /// </summary>
        public Func<IDaoSchemaDefinition, string> SchemaTempPathProvider { get; set; }

        /// <summary>
        /// Gets the C# class name for the specified database table name using the configured name formatter.
        /// </summary>
        /// <param name="tableName">The database table name.</param>
        /// <returns>A mapping from table name to class name.</returns>
        public virtual TableNameToClassName GetClassName(string tableName)
        {
            return new TableNameToClassName { TableName = tableName, ClassName = NameFormatter.FormatClassName(tableName) };
        }

        /// <summary>
        /// Gets the C# property name for the specified column in the specified table using the configured name formatter.
        /// </summary>
        /// <param name="tableName">The database table name.</param>
        /// <param name="columnName">The database column name.</param>
        /// <returns>A mapping from column name to property name.</returns>
        public virtual ColumnNameToPropertyName GetPropertyName(string tableName, string columnName)
        {
            return new ColumnNameToPropertyName { TableName = tableName, ColumnName = columnName, PropertyName = NameFormatter.FormatPropertyName(tableName, columnName) };
        }

        /// <summary>
        /// Creates a <see cref="Column"/> from the database metadata for the specified table and column, applying name formatting and collision avoidance.
        /// </summary>
        /// <param name="tableName">The database table name.</param>
        /// <param name="columnName">The database column name.</param>
        /// <returns>A new <see cref="Column"/> with metadata populated from the database.</returns>
        public Column CreateColumn(string tableName, string columnName)
        {
            FireEvent(PropertyNameFormatting, new DaoSchemaExtractorEventArgs { Column = columnName });
            string propertyName = AvoidCollision(GetPropertyName(tableName, columnName), tableName, columnName);
            propertyName = AvoidNameOfContainingType(propertyName, tableName, columnName);
            NameMap.Set(new ColumnNameToPropertyName { ColumnName = columnName, PropertyName = propertyName, TableName = tableName });
            FireEvent(PropertyNameFormatted, new DaoSchemaExtractorEventArgs { Column = columnName });

            Column column = new Column(columnName, GetColumnDataType(tableName, columnName))
            {
                PropertyName = propertyName,
                DataType = GetColumnDataType(tableName, columnName),
                DbDataType = GetColumnDbDataType(tableName, columnName),
                MaxLength = GetColumnMaxLength(tableName, columnName),
                AllowNull = GetColumnNullable(tableName, columnName)
            };

            return column;
        }

        /// <summary>
        /// Extracts a <see cref="DaoSchemaDefinition"/> from the database using a non-auto-saving schema manager.
        /// </summary>
        /// <returns>The extracted schema definition.</returns>
        public virtual DaoSchemaDefinition Extract()
        {
            DaoSchemaManager schemaManager = new DaoSchemaManager
            {
                AutoSave = false,
                SchemaTempPathProvider = SchemaTempPathProvider
            };
            DaoSchemaDefinition result = Extract(schemaManager);
            return result;
        }

        /// <summary>
        /// Extracts a <see cref="DaoSchemaDefinition"/> from the database using the specified schema manager, populating tables, columns, keys, and foreign keys.
        /// </summary>
        /// <param name="schemaManager">The schema manager to use for building the schema.</param>
        /// <returns>The extracted schema definition.</returns>
        public virtual DaoSchemaDefinition Extract(DaoSchemaManager schemaManager)
        {
            DaoSchemaDefinition result = new DaoSchemaDefinition { Name = GetSchemaName() };
            schemaManager.CurrentSchema = result;

            // GetTableNames
            GetTableNames().Each(tableName =>
            {
                FireEvent(ProcessingTable, new DaoSchemaExtractorEventArgs { Table = tableName });

                FireEvent(ClassNameFormatting, new DaoSchemaExtractorEventArgs { Table = tableName });
                string className = GetClassName(tableName);
                NameMap.Set(new TableNameToClassName { TableName = tableName, ClassName = className });
                FireEvent(ClassNameFormatted, new DaoSchemaExtractorEventArgs { Table = tableName });

                schemaManager.AddTable(tableName, className);//  add each table
                // GetColumnNames
                GetColumnNames(tableName).Each(columnName =>
                {
                    FireEvent(ProcessingColumn, new DaoSchemaExtractorEventArgs { Column = columnName });
                    //  add each column;                     
                    schemaManager.AddColumn(tableName, CreateColumn(tableName, columnName));
                    FireEvent(ProcessingColumnComplete, new DaoSchemaExtractorEventArgs { Column = columnName });
                });

                string keyColumnName = GetKeyColumnName(tableName);
                if (!string.IsNullOrEmpty(keyColumnName))
                {
                    schemaManager.SetKeyColumn(tableName, keyColumnName);
                }

                FireEvent(ProcessingTableComplete, new DaoSchemaExtractorEventArgs { Table = tableName });
            });

            // GetForeignKeyColumns
            GetForeignKeyColumns().Each(fk =>
            {
                FireEvent(ProcessingForeignKey, new DaoSchemaExtractorEventArgs { ForeignKeyColumn = fk });
                //  set each foreignkey
                schemaManager.SetForeignKey(fk.ReferencedTable, fk.TableName, fk.Name, GetKeyColumnName(fk.ReferencedTable), NameFormatter);
                FireEvent(ProcessingForeignComplete, new DaoSchemaExtractorEventArgs { ForeignKeyColumn = fk });
            });
            SaveNameMap(schemaManager);
            SetClassNamesOnColumns(schemaManager);
            return result;
        }

        protected virtual void SaveNameMap(DaoSchemaManager schemaManager)
        {
            NameMap.Save(Path.Combine(RuntimeSettings.ProcessDataFolder, "{0}_NameMap.json".Format(schemaManager.CurrentSchema.Name)));
        }

        protected HashSet<string> GetKeyWords()
        {
            return new HashSet<string>(new[] {
                "ProxyTypeProvider",
                "PostConstructActions",
                "Initializer",
                "GlobalInitializer",
                "Database",
                "AutoDeleteChildren",
                "Validator",
                "GlobalValidator",
                "UniqueFilterProvider",
                "IdValue",
                "KeyColumnName",
                "ForceInsert",
                "ForceUpdate",
                "IsNew",
                "ServiceProvider",
                "DataRow",
                "WriteDelete",
                "WriteCommit",
                "WriteUpdate",
                "WriteInsert",
                "Undo",
                "Undelete",
                "ToJsonSafe",
                "GetUniqueFilter",
                "ConnectionName",
                "UnproxyConnection",
                "ProxyConnection",
                "TableName",
                "GetKeyColumnName",
                "RegisterDaoTypes",
                "GetSchemaTypes",
                "OnInitialize",
                "GetHashCode",
                "Equals",
                "ResetChildren",
                "Validate",
                "Save",
                "Commit",
                "Update",
                "Insert",
                "WriteChildDeletes",
                "Delete",
                "PreLoadChildCollections",
                "ToString",
                "GetType",
                "LoadAll",
                "BatchAll",
                "BatchQuery",
                "GetById",
                "GetByUuid",
                "GetByCuid",
                "Query",
                "Where",
                "GetOneWhere",
                "OneWhere",
                "FirstOneWhere",
                "Top",
                "Count",
                "IsEmpty",
                "Filters",
                "Parameters",
                "Where",
                "Parse",
                "Add",
                "StartsWith",
                "EndsWith",
                "Contains",
                "In",
                "And",
                "Or",
                "Equals",
                "GetHashCode",
                "ToString",
                "GetType"
            });

        }

        private string AvoidNameOfContainingType(string propertyName, string tableName, string columnName)
        {
            string className = NameMap.GetClassName(tableName);
            string result = propertyName;
            if (propertyName.Equals(className))
            {
                result = _namingCollisionHandlers[SchemaExtractorNamingCollisionStrategy](propertyName, tableName, columnName);
                FireEvent(PropertyNameCollisionAvoided, new DaoSchemaExtractorEventArgs { Table = tableName, Column = columnName, Property = propertyName });
            }
            return result;
        }

        private string AvoidCollision(string propertyName, string tableName, string columnName)
        {
            string result = propertyName;
            if (GetKeyWords().Contains(propertyName))
            {
                result = _namingCollisionHandlers[SchemaExtractorNamingCollisionStrategy](tableName, columnName, propertyName);
                FireEvent(PropertyNameCollisionAvoided, new DaoSchemaExtractorEventArgs { Table = tableName, Column = columnName, Property = propertyName });
            }
            return result;
        }

        private void SetClassNamesOnColumns(DaoSchemaManager schemaManager)
        {
            schemaManager.CurrentSchema.Tables.Each(table =>
            {
                table.Columns.Each(col =>
                {
                    col.TableClassName = NameMap.GetClassName(table.Name);
                });
            });
        }


    }
}
