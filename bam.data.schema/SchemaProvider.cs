/*
	Copyright © Bryan Apellanes 2015  
*/
using System.Reflection;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;
using Bam.Net.Data.Schema;
using Bam.Net.Logging;

namespace Bam.Data.Schema
{
    /// <summary>
    /// A class used to generate TypeSchemas and DaoSchemaDefinitions.  A TypeSchema is 
    /// a class that provides database schema like relationship
	/// descriptors for CLR types.
    /// </summary>
    public class SchemaProvider : Loggable, IHasTypeSchemaTempPathProvider, ISchemaProvider
    {
        public const string DefaultSchemaName = "BamSchema";

        public SchemaProvider()
        {
            DefaultDataTypeBehavior = DefaultDataTypeBehaviors.Exclude;
            TypeSchemaTempPathProvider = new SchemaTempPathProvider();
            SchemaManager = new CuidSchemaManager(false);
            TypeSchemaWarnings = new HashSet<ITypeSchemaWarning>();
            TableNameProvider = new EchoTypeTableNameProvider();

            Types = new List<Type>();
        }

        public SchemaProvider(ITypeTableNameProvider tableNameProvider, ISchemaTempPathProvider schemaTempPathProvider)
        {
            DefaultDataTypeBehavior = DefaultDataTypeBehaviors.Exclude;
            TypeSchemaTempPathFuncProvider = schemaTempPathProvider ?? new SchemaTempPathProvider();
            SchemaManager = new CuidSchemaManager(false);
            TypeSchemaWarnings = new HashSet<ITypeSchemaWarning>();
            TableNameProvider = tableNameProvider;

            Types = new List<Type>();
        }

        public DataNamespaces GetDataNamespaces()
        {
            Type? type = Types.FirstOrDefault();
            if (type == null)
            {
                return new DataNamespaces();
            }
            return DataNamespaces.For(type);
        }

        public string GetSchemaNameOrDefault(string? name = null)
        {
            if (Types.Any())
            {
                return GetSchemaName(Types);
            }

            return name ?? DefaultSchemaName;
        }

        public string GetSchemaName(IEnumerable<Type> types)
        {
            Type type = Types.First();
            if (type == null)
            {
                throw new InvalidOperationException($"No types specified, set {nameof(SchemaProvider)}.{nameof(Types)} first.");
            }
            return $"SchemaFor_{type.Namespace}";
        }

        public DaoSchemaManager SchemaManager { get; set; }

        public ITypeTableNameProvider TableNameProvider
        {
            get;
            set;
        }

        Func<IDaoSchemaDefinition, ITypeSchema, string>? _typeSchemaTempPathProvider;
        public Func<IDaoSchemaDefinition, ITypeSchema, string>? TypeSchemaTempPathProvider
        {
            get
            {
                if (_typeSchemaTempPathProvider == null)
                {
                    if (TypeSchemaTempPathFuncProvider != null)
                    {
                        return TypeSchemaTempPathFuncProvider.GetSchemaTempPath;
                    }
                }
                return _typeSchemaTempPathProvider;
            }
            set
            {
                _typeSchemaTempPathProvider = value;
            }
        }

        protected ISchemaTempPathProvider TypeSchemaTempPathFuncProvider
        {
            get;
            set;
        }


        public event EventHandler DifferentTypeNamespacesFound;

        /// <summary>
        /// The event that fires when schema creation begins
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Creating dao schema started: '{SchemaName}'")]
        public event EventHandler CreatingSchemaStarted;

        /// <summary>
        /// The event that fires when type schema creation begins
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Creating type schema started: '{SchemaName}'")]
        public event EventHandler CreatingTypeSchemaStarted;

        /// <summary>
        /// The event that fires when type schema creation completes
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Creating type schema finished: '{SchemaName}'")]
        public event EventHandler CreatingTypeSchemaFinished;

        /// <summary>
        /// The event that fires when dao schema creation begins
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Writing dao schema started: '{SchemaName}'")]
        public event EventHandler WritingDaoSchemaStarted;

        /// <summary>
        /// The event that fires when dao schema creation completes
        /// </summary>
        [Verbosity(VerbosityLevel.Information, SenderMessageFormat = "Writing dao schema finished: '{SchemaName}'")]
        public event EventHandler WritingDaoSchemaFinished;

        /// <summary>
        /// Holds the name of the currently generating
        /// schema
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// If true, an Id column is added to the generated dao tables
        /// </summary>
        public bool AddIdField { get; set; }

        /// <summary>
        /// If true, audit fields Created and Modified is added to the dao tables
        /// </summary>
        public bool AddAuditFields { get; set; }

        /// <summary>
        /// If true, ModifiedBy is added to the dao tables
        /// </summary>
        public bool IncludeModifiedBy { get; set; }

        /// <summary>
        /// If true, CreatedBy is added to the dao tables
        /// </summary>
        public bool IncludeCreatedBy { get; set; }

        /// <summary>
        /// Gets or sets an enum value indicating how to treat
        /// properties whose type is not explicitly supported.
        /// </summary>
        public DefaultDataTypeBehaviors DefaultDataTypeBehavior
        {
            get;
            set;
        }

        public IEnumerable<Type> Types { get; set; }

        public DaoSchemaDefinitionCreateResult CreateDaoSchemaDefinition(string schemaName = null)
        {
            Args.ThrowIf(!Types.Any(), "No types specified");
            return CreateDaoSchemaDefinition(Types, schemaName);
        }

        public DaoSchemaDefinitionCreateResult CreateDaoSchemaDefinition(IEnumerable<Type> types, string schemaName = null)
        {
            SchemaName = schemaName ?? "null";
            FireEvent(CreatingSchemaStarted, EventArgs.Empty);

            AddAugmentations();

            FireEvent(CreatingTypeSchemaStarted, EventArgs.Empty);
            TypeSchema typeSchema = CreateTypeSchema(types, schemaName);
            FireEvent(CreatingTypeSchemaFinished, EventArgs.Empty);

            schemaName = schemaName ?? $"_{typeSchema.Name}_";
            SchemaManager.SetSchema(schemaName, false); //TODO: enable more granular manipulation of schema file path in schema manager by giving it a PathProvider property
            SchemaName = schemaName;

            FireEvent(WritingDaoSchemaStarted, EventArgs.Empty);
            List<KeyColumn> missingKeyColumns = new List<KeyColumn>();
            List<ForeignKeyColumn> missingForeignKeyColumns = new List<ForeignKeyColumn>();
            WriteDaoSchema(typeSchema, SchemaManager, missingKeyColumns, missingForeignKeyColumns, TableNameProvider);
            FireEvent(WritingDaoSchemaFinished, EventArgs.Empty);

            DaoSchemaDefinitionCreateResult result = new DaoSchemaDefinitionCreateResult(SchemaManager.GetCurrentSchema(), typeSchema, missingKeyColumns.ToArray(), missingForeignKeyColumns.ToArray()) { TypeSchemaWarnings = TypeSchemaWarnings };

            return result;
        }

        public HashSet<ITypeSchemaWarning> TypeSchemaWarnings { get; set; }

        public TypeSchema CreateTypeSchema()
        {
            return CreateTypeSchema(Types);
        }

        public TypeSchema CreateTypeSchema(string name, params Type[] types)
        {
            return CreateTypeSchema(types, name);
        }

        /// <summary>
        /// Create a TypeSchema from the specified types
        /// </summary>
        /// <param name="types"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public TypeSchema CreateTypeSchema(IEnumerable<Type> types, string name = null)
        {
            CheckBaseNamespaces(types);
            SubscribeToTypeSchemaWarnings();

            HashSet<Type> tableTypes = new HashSet<Type>();
            HashSet<ITypeFk> foreignKeyTypes = new HashSet<ITypeFk>();
            HashSet<ITypeXref> xrefTypes = new HashSet<ITypeXref>();

            foreach (Type type in types)
            {
                foreach (Type tableType in GetTableTypes(type))
                {
                    tableTypes.Add(tableType);
                }
            }

            foreach (ITypeFk typeFk in GetForeignKeyTypes(tableTypes))
            {
                foreignKeyTypes.Add(typeFk);
            }

            foreach (ITypeXref xref in GetXrefTypes(tableTypes))
            {
                xrefTypes.Add(xref);
            }

            return new TypeSchema { Name = GetSchemaNameOrDefault(name), Tables = tableTypes, ForeignKeys = foreignKeyTypes, Xrefs = xrefTypes, DefaultDataTypeBehavior = DefaultDataTypeBehavior, Warnings = TypeSchemaWarnings };
        }

        protected internal virtual void WriteDaoSchema(TypeSchema typeSchema, DaoSchemaManager schemaManager, List<KeyColumn> missingKeyColumns = null, List<ForeignKeyColumn> missingForeignKeyColumns = null, ITypeTableNameProvider tableNameProvider = null)
        {
            AddSchemaTables(typeSchema, schemaManager, tableNameProvider);

            HashSet<ITypeFk> foreignKeyTypes = typeSchema.ForeignKeys;
            HashSet<ITypeXref> xrefTypes = typeSchema.Xrefs;
            // accounting for missing columns
            // loop primary keys and fks separately to ensure 
            // missing keys get recorded prior to trying to add the
            // fks
            foreach (TypeFk foreignKey in foreignKeyTypes)
            {
                TypeSchemaPropertyInfo? keyInfo = foreignKey.PrimaryKeyProperty as TypeSchemaPropertyInfo;
                if (keyInfo != null)
                {
                    KeyColumn key = keyInfo.ToKeyColumn();
                    schemaManager.AddColumn(key.TableName, key);
                    schemaManager.SetKeyColumn(key.TableName, key.Name);
                    missingKeyColumns?.Add(key);
                }
            }

            foreach (TypeFk foreignKey in foreignKeyTypes)
            {
                TypeSchemaPropertyInfo? fkInfo = foreignKey.ForeignKeyProperty as TypeSchemaPropertyInfo;
                if (fkInfo != null)
                {
                    PropertyInfo keyProperty = GetKeyProperty(foreignKey.PrimaryKeyType, missingKeyColumns);
                    string referencedKeyName = keyProperty.Name;

                    ForeignKeyColumn fk = fkInfo.ToForeignKeyColumn(tableNameProvider);
                    fk.AllowNull = true;
                    schemaManager.AddColumn(fk.TableName, fk);
                    schemaManager.SetForeignKey(fk.ReferencedTable, fk.TableName, fk.Name, referencedKeyName);
                    missingForeignKeyColumns?.Add(fk);
                }
            }
            // /end - accounting for missing columns

            foreach (TypeFk foreignKey in foreignKeyTypes)
            {
                schemaManager.SetForeignKey(
                    GetTableNameForType(foreignKey.PrimaryKeyType, tableNameProvider),
                    GetTableNameForType(foreignKey.ForeignKeyType, tableNameProvider),
                    foreignKey.ForeignKeyProperty.Name);
            }

            foreach (TypeXref xref in xrefTypes)
            {
                schemaManager.SetXref(GetTableNameForType(xref.Left, tableNameProvider), GetTableNameForType(xref.Right, tableNameProvider));
            }
        }

        protected virtual void AddSchemaTables(TypeSchema typeSchema, DaoSchemaManager schemaManager, ITypeTableNameProvider tableNameProvider = null)
        {
            tableNameProvider = tableNameProvider ?? new EchoTypeTableNameProvider();
            foreach (Type tableType in typeSchema.Tables)
            {
                string tableName = GetTableNameForType(tableType, tableNameProvider);
                schemaManager.AddTable(tableName);
                schemaManager.ExecutePreColumnAugmentations(tableName);
                AddPropertyColumns(tableType, schemaManager, typeSchema.DefaultDataTypeBehavior);
                schemaManager.ExecutePostColumnAugmentations(tableName);
            }
        }

        protected internal HashSet<Type> GetTableTypes(Type type)
        {
            HashSet<Type> results = new HashSet<Type> { type };
            Traverse(type, results);
            return results;
        }

        protected internal HashSet<TypeFk> GetForeignKeyTypes(Type type)
        {
            return GetForeignKeyTypes(new Type[] { type });
        }

        protected internal HashSet<TypeFk> GetForeignKeyTypes(IEnumerable<Type> types)
        {
            HashSet<TypeFk> results = new HashSet<TypeFk>();
            foreach (Type type in types)
            {
                foreach (TypeFk fk in GetReferencingForeignKeyTypesFor(type))
                {
                    results.Add(fk);
                }
            }

            return results;
        }

        protected internal HashSet<TypeXref> GetXrefTypes(Type type)
        {
            return GetXrefTypes(new Type[] { type });
        }

        protected internal HashSet<TypeXref> GetXrefTypes(IEnumerable<Type> types)
        {
            HashSet<TypeXref> results = new HashSet<TypeXref>();
            foreach (Type type in types)
            {
                foreach (TypeXref xref in GetXrefTypesFor(type))
                {
                    results.Add(xref);
                }
            }
            return results;
        }

        /// <summary>
        /// Get the properties where the type of the
        /// property is of a type that has a property that is
        /// an enumerable of the type specified
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected internal IEnumerable<TypeXref> GetXrefTypesFor(Type type)
        {
            HashSet<TypeXref> xrefTypes = new HashSet<TypeXref>();
            foreach (PropertyInfo property in type.GetProperties().Where(p => p.CanWrite))
            {
                if (!_daoPrimitives.Contains(property.PropertyType))
                {
                    Type enumerableType = property.GetEnumerableType();
                    if (enumerableType != null)
                    {
                        if (AreXrefs(type, enumerableType, out PropertyInfo leftEnumerable, out PropertyInfo rightEnumerable))
                        {
                            xrefTypes.Add(new TypeXref { Left = type, Right = enumerableType, LeftCollectionProperty = leftEnumerable, RightCollectionProperty = rightEnumerable });
                        }
                    }
                }
            }

            return xrefTypes;
        }

        protected internal static bool AreXrefs(Type left, Type right)
        {
            return AreXrefs(left, right, out _, out _);
        }

        protected internal static bool AreXrefs(Type left, Type right, out PropertyInfo leftEnumerable, out PropertyInfo rightEnumerable)
        {
            rightEnumerable = null;
            return left.HasEnumerableOfMe(right, out leftEnumerable) && right.HasEnumerableOfMe(left, out rightEnumerable);
        }

        /// <summary>
        /// A string representation of the UtcNow at the time
        /// of reference.  <see cref="Net.Instant" />
        /// </summary>
        public string Instant => new Instant(DateTime.UtcNow).ToString();

        public string Message { get; set; }

        /// <summary>
        /// The event that occurs when a Type is found in the current
        /// TypeSchema hierarchy with no Key property specified (the Type's key is determined
        /// by whether a property has the KeyAttribute custom attribute or
        /// the name of "Id")
        /// </summary>
        [Verbosity(VerbosityLevel.Warning, SenderMessageFormat = "[{Instant}]:: KeyPropertyNotFound: {Message}\r\n")]
        public event EventHandler KeyPropertyNotFound;

        /// <summary>
        /// The event that occurs when a Type is found in the current
        /// TypeSchema hierarchy with an IEnumerable&lt;T&gt; property where the underlying type of
        /// the IEnumerable doesn't have a property referencing
        /// the current Type's key (the Type's key is determined
        /// by whether a property has the KeyAttribute custom attribute or
        /// the name of "Id")
        /// </summary>
        [Verbosity(VerbosityLevel.Warning, SenderMessageFormat = "[{Instant}]:: ReferencingPropertyNotFound: {Message}\r\n")]
        public event EventHandler ReferencingPropertyNotFound;

        /// <summary>
        /// The event that occurs when a Type is found in the current
        /// TypeSchema hierarchy with an IEnumerable&lt;T&gt; property where the underlying type of
        /// the IEnumerable doesn't have a property of the parent Type to hold the instance of
        /// the parent.
        /// </summary>
        [Verbosity(VerbosityLevel.Warning, SenderMessageFormat = "[{Instant}]:: ChildParentPropertyNotFound: {Message}\r\n")]
        public event EventHandler ChildParentPropertyNotFound;

        /// <summary>
        /// Get the types for each IEnumerable property of the specified type
        /// </summary>
        /// <param name="parentType"></param>
        /// <returns></returns>
        protected internal IEnumerable<TypeFk> GetReferencingForeignKeyTypesFor(Type parentType)
        {
            HashSet<TypeFk> results = new HashSet<TypeFk>();
            foreach (PropertyInfo property in parentType.GetProperties().Where(p => p.CanWrite))
            {
                Type propertyType = property.PropertyType;
                if (propertyType != typeof(byte[]) &&
                    propertyType != typeof(string) &&
                    property.IsEnumerable() &&
                    property.GetEnumerableType() != typeof(string) &&
                    !AreXrefs(parentType, property.GetEnumerableType()))
                {
                    PropertyInfo keyProperty = GetKeyProperty(parentType);
                    Type foreignKeyType = property.GetEnumerableType();
                    PropertyInfo referencingProperty = null;
                    if (keyProperty == null)
                    {
                        Message = "KeyProperty not found for type {0}".Format(parentType.FullName);
                        FireEvent(KeyPropertyNotFound, new TypeSchemaWarningEventArgs() { Warning = Schema.TypeSchemaWarnings.KeyPropertyNotFound, ParentType = parentType });
                        keyProperty = new TypeSchemaPropertyInfo("Id", parentType, TableNameProvider);
                    }

                    string referencingPropertyName = "{0}{1}".Format(parentType.Name, keyProperty.Name);
                    referencingProperty = foreignKeyType.GetProperty(referencingPropertyName);

                    if (referencingProperty == null)
                    {
                        Message = "Referencing property not found {0}: Parent type ({1}), ForeignKeyType ({2})".Format(referencingPropertyName, parentType.FullName, foreignKeyType.FullName);
                        FireEvent(ReferencingPropertyNotFound, new TypeSchemaWarningEventArgs() { Warning = Schema.TypeSchemaWarnings.ReferencingPropertyNotFound, ParentType = parentType, ForeignKeyType = foreignKeyType });
                        referencingProperty = new TypeSchemaPropertyInfo(referencingPropertyName, parentType, foreignKeyType, TableNameProvider);
                    }

                    PropertyInfo childParentProperty = foreignKeyType.GetProperty(parentType.Name);
                    if (childParentProperty == null)
                    {
                        Message = "ChildParentProperty was not found {0}.{1}: Parent type({2}), ForeignKeyType ({3})".Format(foreignKeyType.Name, parentType.Name, parentType.FullName, foreignKeyType.FullName);
                        FireEvent(ChildParentPropertyNotFound, new TypeSchemaWarningEventArgs() { Warning = Schema.TypeSchemaWarnings.ChildParentPropertyNotFound, ParentType = parentType, ForeignKeyType = foreignKeyType });
                        childParentProperty = new TypeSchemaPropertyInfo(parentType.Name, foreignKeyType, TableNameProvider);
                    }

                    results.Add(new TypeFk
                    {
                        PrimaryKeyType = parentType,
                        PrimaryKeyProperty = keyProperty,
                        ForeignKeyType = foreignKeyType,
                        ForeignKeyProperty = referencingProperty,
                        ChildParentProperty = childParentProperty,
                        CollectionProperty = property
                    });
                }
            }

            return results;
        }

        protected internal static PropertyInfo GetKeyProperty(Type type, List<KeyColumn> keyColumnsToCheck = null, ITypeTableNameProvider tableNameProvider = null)
        {
            tableNameProvider = tableNameProvider ?? new EchoTypeTableNameProvider();
            PropertyInfo keyProperty = type.GetFirstProperyWithAttributeOfType<KeyAttribute>();
            if (keyProperty == null)
            {
                keyProperty = type.GetProperty("Id");
            }

            if (keyProperty == null && keyColumnsToCheck != null)
            {
                KeyColumn keyColumn = keyColumnsToCheck.FirstOrDefault(kc => kc.TableName.Equals(GetTableNameForType(type, tableNameProvider)));
                if (keyColumn != null)
                {
                    keyProperty = new TypeSchemaPropertyInfo(keyColumn.Name, type, tableNameProvider);
                }
            }
            return keyProperty;
        }

        protected internal static string GetTableNameForType(Type type, ITypeTableNameProvider tableNameProvider = null)
        {
            tableNameProvider = tableNameProvider ?? new EchoTypeTableNameProvider();
            return tableNameProvider.GetTableName(type);
        }

        static readonly List<Type> _daoPrimitives = new List<Type> { typeof(bool), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal), typeof(byte[]), typeof(DateTime), typeof(string) };

        protected internal static DataTypes GetColumnDataType(PropertyInfo property)
        {
            DataTypes dataType = DataTypes.Default;
            Type propertyType = property.PropertyType;
            if (propertyType == typeof(bool) ||
                propertyType == typeof(bool?))
            {
                dataType = DataTypes.Boolean;
            }
            else if (propertyType == typeof(int) ||
                propertyType == typeof(int?))
            {
                dataType = DataTypes.Int;
            }
            else if (propertyType == typeof(long) ||
                propertyType == typeof(long?))
            {
                dataType = DataTypes.Long;
            }
            else if (propertyType == typeof(ulong) ||
               propertyType == typeof(ulong?))
            {
                dataType = DataTypes.ULong;
            }
            else if (propertyType == typeof(decimal) ||
                propertyType == typeof(decimal?))
            {
                dataType = DataTypes.Decimal;
            }
            else if (propertyType == typeof(byte[]) ||
                propertyType == typeof(byte?[]))
            {
                dataType = DataTypes.ByteArray;
            }
            else if (propertyType == typeof(DateTime) ||
                propertyType == typeof(DateTime?))
            {
                dataType = DataTypes.DateTime;
            }
            else if (propertyType == typeof(string))
            {
                dataType = DataTypes.String;
            }
            return dataType;
        }

        protected virtual void AddPropertyColumns(Type type, DaoSchemaManager schemaManager, DefaultDataTypeBehaviors defaultDataTypeBehavior, ITypeTableNameProvider tableNameProvider = null)
        {
            string tableName = GetTableNameForType(type, tableNameProvider);
            foreach (PropertyInfo property in type.GetProperties().Where(p => p.CanWrite))
            {
                AddPropertyColumn(schemaManager, defaultDataTypeBehavior, tableName, property);
            }
        }

        protected virtual void AddPropertyColumn(DaoSchemaManager schemaManager, DefaultDataTypeBehaviors defaultDataTypeBehavior, string tableName, PropertyInfo property)
        {
            DataTypes dataType = GetColumnDataType(property);
            if (dataType == DataTypes.Default)
            {
                switch (defaultDataTypeBehavior)
                {
                    case DefaultDataTypeBehaviors.Invalid:
                    case DefaultDataTypeBehaviors.Exclude:
                        break;
                    case DefaultDataTypeBehaviors.IncludeAsString:
                        AddSchemaColumn(schemaManager, tableName, property, DataTypes.String);
                        break;
                    case DefaultDataTypeBehaviors.IncludeAsByteArray:
                        AddSchemaColumn(schemaManager, tableName, property, DataTypes.ByteArray);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                AddSchemaColumn(schemaManager, tableName, property, dataType);
            }
        }

        protected static void AddSchemaColumn(DaoSchemaManager schemaManager, string tableName, PropertyInfo property, DataTypes dataType)
        {
            if (!property.IsEnumerable() || property.PropertyType == typeof(string))
            {
                Column columnToAdd = new Column(property.Name.LettersOnly(), dataType);
                schemaManager.AddColumn(tableName, columnToAdd);
                if (property.HasCustomAttributeOfType<KeyAttribute>() || property.Name.Equals("Id"))
                {
                    schemaManager.SetKeyColumn(tableName, columnToAdd.Name);
                }
            }
        }

        private bool _subscribedToTypeSchemaWarnings;
        private void SubscribeToTypeSchemaWarnings()
        {
            if (!_subscribedToTypeSchemaWarnings)
            {
                _subscribedToTypeSchemaWarnings = true;

                void Handler(object? o, EventArgs a)
                {
                    ITypeSchemaWarning warning = TypeSchemaWarning.FromEventArgs((TypeSchemaWarningEventArgs)a);
                    TypeSchemaWarnings.Add(warning);
                }

                KeyPropertyNotFound += Handler;
                ReferencingPropertyNotFound += Handler;
                ChildParentPropertyNotFound += Handler;
                DifferentTypeNamespacesFound += Handler;
            }
        }

        private void Traverse(Type type, HashSet<Type> results)
        {
            Queue<Type> queue = new Queue<Type>();
            queue.Enqueue(type);
            while (queue.Count > 0)
            {
                Type current = queue.Dequeue();
                results.Add(current);
                foreach (TypeFk typeFk in GetReferencingForeignKeyTypesFor(current))
                {
                    if (!results.Contains(typeFk.PrimaryKeyType))
                    {
                        queue.Enqueue(typeFk.PrimaryKeyType);
                    }
                    if (!results.Contains(typeFk.ForeignKeyType))
                    {
                        queue.Enqueue(typeFk.ForeignKeyType);
                    }
                }

                foreach (TypeXref xref in GetXrefTypesFor(current))
                {
                    if (!results.Contains(xref.Left))
                    {
                        queue.Enqueue(xref.Left);
                    }

                    if (!results.Contains(xref.Right))
                    {
                        queue.Enqueue(xref.Right);
                    }
                }
            }
        }

        private void AddAugmentations()
        {
            if (AddIdField)
            {
                SchemaManager.PreColumnAugmentations.Add(new AddIdKeyColumnAugmentation());
            }

            if (AddAuditFields)
            {
                SchemaManager.PostColumnAugmentations.Add(new AddAuditColumnsAugmentation { IncludeModifiedBy = IncludeModifiedBy, IncludeCreatedBy = IncludeCreatedBy });
            }
        }

        /// <summary>
        /// Fires warnings if the specified types are in different namespaces.
        /// </summary>
        /// <param name="types"></param>
        private void CheckBaseNamespaces(IEnumerable<Type> types)
        {
            HashSet<string> namespaces = new HashSet<string>();
            foreach (Type type in types)
            {
                namespaces.Add(type.Namespace);
            }

            if (namespaces.Count > 1)
            {
                FireEvent(DifferentTypeNamespacesFound, new TypeSchemaWarningEventArgs() { Warning = Schema.TypeSchemaWarnings.DifferentTypeNamespacesFound, Namespaces = namespaces.ToArray() });
            }
        }
    }
}
