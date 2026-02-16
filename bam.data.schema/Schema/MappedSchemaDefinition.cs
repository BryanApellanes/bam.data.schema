/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Associates a <see cref="DaoSchemaDefinition"/> with a <see cref="SchemaNameMap"/>, enabling class and property name mapping during code generation.
    /// </summary>
    public class MappedSchemaDefinition
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MappedSchemaDefinition"/> with a default file path.
        /// </summary>
        public MappedSchemaDefinition() : this("./{0}.lzs.json".Format(typeof(MappedSchemaDefinition).Name)) { }

        /// <summary>
        /// Initializes a new instance of <see cref="MappedSchemaDefinition"/> with the specified file path.
        /// </summary>
        /// <param name="filePath">The path to save the mapped schema definition to.</param>
        public MappedSchemaDefinition(string filePath)
        {
            this.SchemaNameMap = new SchemaNameMap();
            this.SchemaDefinition = new DaoSchemaDefinition();
            this.FilePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MappedSchemaDefinition"/> with the specified schema definition and name map.
        /// </summary>
        /// <param name="definition">The DAO schema definition.</param>
        /// <param name="nameMap">The name map for class and property name resolution.</param>
        public MappedSchemaDefinition(IDaoSchemaDefinition definition, SchemaNameMap nameMap)
            : this()
        {
            this.SchemaNameMap = nameMap;
            this.SchemaDefinition = definition;
        }

        /// <summary>
        /// Gets or sets the file path for serialization.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the mapping between database names and C# class/property names.
        /// </summary>
        public SchemaNameMap SchemaNameMap { get; set; }

        /// <summary>
        /// Gets or sets the DAO schema definition.
        /// </summary>
        public IDaoSchemaDefinition SchemaDefinition { get; set; }

        /// <summary>
        /// Loads a <see cref="MappedSchemaDefinition"/> from the specified file path.
        /// </summary>
        /// <param name="filePath">The path to load from.</param>
        /// <returns>The loaded mapped schema definition.</returns>
        public static MappedSchemaDefinition Load(string filePath)
        {
            return Load(new FileInfo(filePath));
        }
        /// <summary>
        /// Loads a <see cref="MappedSchemaDefinition"/> from the specified file.
        /// </summary>
        /// <param name="file">The file to load from.</param>
        /// <returns>The loaded mapped schema definition.</returns>
        public static MappedSchemaDefinition Load(FileInfo file)
        {
            MappedSchemaDefinition def = file.FromJsonFile<MappedSchemaDefinition>();
            def.FilePath = file.FullName;
            return def;
        }

        /// <summary>
        /// Saves this mapped schema definition to the current <see cref="FilePath"/>.
        /// </summary>
        public void Save()
        {
            Save(this.FilePath);
        }

        /// <summary>
        /// Saves this mapped schema definition to the specified file path.
        /// </summary>
        /// <param name="filePath">The file path to save to.</param>
        public void Save(string filePath)
        {
            Save(new FileInfo(filePath));
        }

        /// <summary>
        /// Applies the name map to the schema definition, setting class and property names on all tables and columns.
        /// </summary>
        /// <returns>The schema definition with mapped class and property names.</returns>
        public IDaoSchemaDefinition MapSchemaClassAndPropertyNames()
        {
            return MapSchemaClassAndPropertyNames(SchemaNameMap, SchemaDefinition);
        }

        /// <summary>
        /// Saves this mapped schema definition to the specified file, applying name mappings first.
        /// </summary>
        /// <param name="file">The file to save to.</param>
        public void Save(FileInfo file)
        {
            SchemaDefinition = MapSchemaClassAndPropertyNames();
            this.FilePath = file.FullName;
            this.ToJsonFile(file);
        }

        /// <summary>
        /// Applies the specified name map to the schema definition, setting class and property names on all tables and columns.
        /// </summary>
        /// <param name="nameMap">The name map to apply.</param>
        /// <param name="schema">The schema definition to update.</param>
        /// <returns>The schema definition with mapped class and property names.</returns>
        public static IDaoSchemaDefinition MapSchemaClassAndPropertyNames(SchemaNameMap nameMap, IDaoSchemaDefinition schema)
        {
            DaoSchemaManager mgr = new DaoSchemaManager(schema) {AutoSave = false};
            Parallel.ForEach(nameMap.TableNamesToClassNames, (map) =>
            {
                mgr.SetTableClassName(map.TableName, map.ClassName);
            });
            Parallel.ForEach(schema.Tables, (table) =>
            {
                Parallel.ForEach(table.Columns, column => mgr.SetColumnPropertyName(table.Name, column.Name, nameMap.GetPropertyName(table.Name, column.Name)));
            });  
            return mgr.CurrentSchema;
        }
    }
}
