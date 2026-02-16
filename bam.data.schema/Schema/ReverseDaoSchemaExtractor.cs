using System.Reflection;
using Bam.Data.Repositories;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Extracts a schema definition from compiled DAO types in an assembly, reversing the code generation process to reconstruct the schema.
    /// </summary>
    public class ReverseDaoSchemaExtractor : DaoSchemaExtractor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReverseDaoSchemaExtractor"/> for the specified assembly and namespace.
        /// </summary>
        /// <param name="assembly">The assembly containing compiled DAO types.</param>
        /// <param name="nameSpace">The namespace to search for DAO types within the assembly.</param>
        public ReverseDaoSchemaExtractor(Assembly assembly, string nameSpace)
        {
            Assembly = assembly;
            Namespace = nameSpace;
            DataTypeTranslator = new DataTypeTranslator();
        }
        
        /// <summary>
        /// Gets or sets the assembly containing the DAO types to extract schema from.
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// Gets or sets the namespace to search for DAO types.
        /// </summary>
        public string Namespace { get; set; }

        private Dictionary<string, Type> _daoTypes;
        private Dictionary<string, List<ColumnAttribute>> _columnAttributes;
        protected DataTypeTranslator DataTypeTranslator { get; }

        private bool _analyzed;
        private readonly object _analyzeLock = new object();
        /// <summary>
        /// Analyzes the assembly to discover DAO types and their column attributes. Must be called before extraction.
        /// </summary>
        public void Analyze()
        {
            if (!_analyzed)
            {
                lock (_analyzeLock)
                {
                    if (_daoTypes == null)
                    {
                        Args.ThrowIfNull(Assembly, nameof(Assembly));
                        Args.ThrowIfNullOrEmpty(Namespace, nameof(Namespace));

                        _daoTypes = new Dictionary<string, Type>();
                
                        Assembly
                            .GetTypes()
                            .Where(type =>
                                type.Namespace != null && 
                                type.Namespace.Equals(Namespace) &&
                                type.ExtendsType(typeof(Dao)) &&
                                type.HasCustomAttributeOfType<TableAttribute>())
                            .Each(type => _daoTypes.Add(Dao.TableName(type), type));
                    }

                    if (_columnAttributes == null)
                    {
                        _columnAttributes = new Dictionary<string, List<ColumnAttribute>>();
                        foreach (Type daoType in _daoTypes.Values)
                        {
                            _columnAttributes.Add(Dao.TableName(daoType), 
                                daoType.GetProperties()
                                    .Where(prop=> prop.HasCustomAttributeOfType<ColumnAttribute>())
                                    .Select(prop=> prop.GetCustomAttributeOfType<ColumnAttribute>())
                                    .ToList());
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public override DaoSchemaDefinition Extract()
        {
            if (!_analyzed)
            {
                Analyze();
            }
            return base.Extract();
        }

        /// <inheritdoc />
        public override string GetSchemaName()
        {
            HashSet<string> uniqueSchemaNames = new HashSet<string>();
            foreach (Type daoType in _daoTypes.Values)
            {
                uniqueSchemaNames.Add(Dao.ConnectionName(daoType));
            }

            if (uniqueSchemaNames.Count > 1)
            {
                throw new InvalidOperationException($"Multiple schema names were found in the specified assembly ({Assembly.FullName}) and namespace ({Namespace}): {string.Join(", ", uniqueSchemaNames.ToArray())}");
            }
            else if (uniqueSchemaNames.Count == 0)
            {
                throw new InvalidOperationException($"No dao types were found in the specified namespace ({Namespace}) of the specified assembly ({Assembly.FullName}).");
            }

            return uniqueSchemaNames.FirstOrDefault();
        }

        /// <inheritdoc />
        public override string[] GetTableNames()
        {
            return _daoTypes.Keys.ToArray();
        }

        /// <inheritdoc />
        public override string GetKeyColumnName(string tableName)
        {
            return Dao.GetKeyColumnName(_daoTypes[tableName]);
        }

        /// <inheritdoc />
        public override string[] GetColumnNames(string tableName)
        {
            return _columnAttributes[tableName].Select(c => c.Name).ToArray();
        }

        /// <inheritdoc />
        public override DataTypes GetColumnDataType(string tableName, string columnName)
        {
            ColumnAttribute columnAttribute = GetColumnAttribute(tableName, columnName);

            return DataTypeTranslator.TranslateDataType(columnAttribute.DbDataType);
        }
        
        /// <inheritdoc />
        public override string GetColumnDbDataType(string tableName, string columnName)
        {
            return GetColumnAttribute(tableName, columnName).DbDataType;
        }

        /// <inheritdoc />
        public override string GetColumnMaxLength(string tableName, string columnName)
        {
            return GetColumnAttribute(tableName, columnName).MaxLength;
        }

        /// <inheritdoc />
        public override bool GetColumnNullable(string tableName, string columnName)
        {
            return GetColumnAttribute(tableName, columnName).AllowNull;
        }

        /// <inheritdoc />
        public override ForeignKeyColumn[] GetForeignKeyColumns()
        {
            List<ForeignKeyColumn> foreignKeyColumns = new List<ForeignKeyColumn>();
            foreach (string tableName in _columnAttributes.Keys)
            {
                List<ColumnAttribute> columnAttributes = _columnAttributes[tableName];
                foreignKeyColumns.AddRange(columnAttributes.OfType<ForeignKeyAttribute>().Select(ColumnFromAttribute));
            }

            return foreignKeyColumns.ToArray();
        }
        
        protected override void SetConnectionName(string connectionString)
        {
            // no op
        }

        private ForeignKeyColumn ColumnFromAttribute(ForeignKeyAttribute attribute)
        {
            return new ForeignKeyColumn()
            {
                TableName = attribute.Table,
                Name = attribute.Name,
                AllowNull = attribute.AllowNull,
                ReferenceName = attribute.ForeignKeyName,
                ReferencedKey = attribute.ReferencedKey,
                ReferencedTable = attribute.ReferencedTable
            };
        }
        
        private ColumnAttribute GetColumnAttribute(string tableName, string columnName)
        {
            ColumnAttribute columnAttribute = _columnAttributes[tableName].FirstOrDefault(c => c.Name.Equals(columnName));
            if (columnAttribute == null)
            {
                throw new InvalidOperationException($"Column not found {tableName}.{columnName}");
            }

            return columnAttribute;
        }
    }
}