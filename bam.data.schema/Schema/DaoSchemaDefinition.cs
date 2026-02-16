/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Xml.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Represents a complete DAO schema definition containing tables, columns, foreign keys, and cross-reference tables. Can be serialized to/from JSON.
    /// </summary>
    public class DaoSchemaDefinition : IDaoSchemaDefinition
    {
        Dictionary<string, ITable> _tables = new Dictionary<string, ITable>();
        Dictionary<string, ColumnAttribute> _columns = new Dictionary<string, ColumnAttribute>();

        public DaoSchemaDefinition()
        {
            this.Name = "Default";
            this.DbType = "UnSpecified";
        }

        public DaoSchemaDefinition(string name): this()
        {
            Name = name;
            File = $"{RuntimeSettings.ProcessDataFolder}\\{name}_schema_definition.json";
        }

        /// <summary>
        /// Gets or sets the type of the database that this SchemaDefinition was
        /// extracted from.  May be null.
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// Gets or sets the name of the current SchemaDefinition.
        /// </summary>
        public string Name { get; set; }

        FileInfo _file;
        /// <summary>
        /// Gets or sets the file path where this schema definition is stored. Setting this value creates the parent directory if it does not exist.
        /// </summary>
        [Exclude]
        public string File
        {
            get
            {
                if (_file != null)
                {
                    return _file.FullName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _file = new FileInfo(Path.Combine(RuntimeSettings.ProcessDataFolder, this.Name));
                }
                else
                {
                    _file = new FileInfo(value);
                    if (_file.Directory?.Exists == false)
                    {
                        _file.Directory.Create();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified table from the schema.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        public void RemoveTable(ITable table)
        {
            RemoveTable(table.Name);
        }

        /// <summary>
        /// Removes the table with the specified name from the schema.
        /// </summary>
        /// <param name="tableName">The name of the table to remove.</param>
        public void RemoveTable(string tableName)
        {
            if (this._tables.ContainsKey(tableName))
            {
                this._tables.Remove(tableName);
            }
        }

        /// <summary>
        /// Gets or sets the tables in this schema definition.
        /// </summary>
        public ITable[] Tables
        {
            get
            {
				List<ITable> tables = new List<ITable>();
				tables.AddRange(this._tables.Values);
				return tables.ToArray();
            }
            set
            {
                this._tables.Clear();
                foreach (Table table in value)
                {
                    if (string.IsNullOrEmpty(table.ConnectionName))
                    {
                        table.ConnectionName = this.Name;
                    }
					if (!this._tables.TryAdd(table.Name, table))
					{
                        throw Args.Exception<InvalidOperationException>("Table named {0} defined more than once", table.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the table with the specified name, or null if not found.
        /// </summary>
        /// <param name="tableName">The name of the table to retrieve.</param>
        /// <returns>The table if found; otherwise null.</returns>
        public ITable GetTable(string tableName)
        {
            ITable table = null;
			if (this._tables.TryGetValue(tableName, out var table1))
			{
				table = table1;
			}
            return table;
        }

        readonly List<IForeignKeyColumn> _foreignKeys = new List<IForeignKeyColumn>();
        /// <summary>
        /// Gets or sets the foreign key columns defined in this schema.
        /// </summary>
        public IForeignKeyColumn[] ForeignKeys
        {
            get => this._foreignKeys.ToArray();
            set
            {
                this._foreignKeys.Clear();
                this._foreignKeys.AddRange(value);
            }
        }

        Dictionary<string, IXrefTable> _xrefs = new Dictionary<string, IXrefTable>();
        /// <summary>
        /// Gets or sets the cross-reference (many-to-many) tables in this schema.
        /// </summary>
        public IXrefTable[] Xrefs
        {
            get => _xrefs.Values.ToArray();
            set
            {
                this._xrefs = value.ToDictionary<IXrefTable, string>(x => x.Name); // to Dictionary key by name
            }
        }

        /// <summary>
        /// Gets the cross-reference info entries where the specified table is the left side.
        /// </summary>
        /// <param name="tableName">The table name to find left cross-references for.</param>
        /// <returns>An array of cross-reference info entries.</returns>
        public IXrefInfo[] LeftXrefsFor(string tableName)
        {
            return (from xref in Xrefs
                    where xref.Left.Equals(tableName)
                    select xref).Select(x => new XrefInfo(tableName, x.Name, x.Right)).ToArray();
        }

        /// <summary>
        /// Gets the cross-reference info entries where the specified table is the right side.
        /// </summary>
        /// <param name="tableName">The table name to find right cross-references for.</param>
        /// <returns>An array of cross-reference info entries.</returns>
        public IXrefInfo[] RightXrefsFor(string tableName)
        {
            return (from xref in Xrefs
                    where xref.Right.Equals(tableName)
                    select xref).Select(x => new XrefInfo(tableName, x.Name, x.Left)).ToArray();
        }

        /// <summary>
        /// Gets the cross-reference table with the specified name, or null if not found.
        /// </summary>
        /// <param name="tableName">The name of the xref table to retrieve.</param>
        /// <returns>The xref table if found; otherwise null.</returns>
        public IXrefTable GetXref(string tableName)
        {
            IXrefTable result = null;
            if (this._xrefs.ContainsKey(tableName))
            {
                result = this._xrefs[tableName];
            }

            return result;
        }

        /// <summary>
        /// Adds or updates a cross-reference table in this schema definition.
        /// </summary>
        /// <param name="xref">The cross-reference table to add or update.</param>
        /// <returns>The result of the operation.</returns>
        public IDaoSchemaManagerResult AddXref(IXrefTable xref)
        {
            IDaoSchemaManagerResult r = new DaoSchemaManagerResult($"XrefTable {xref.Name} was added.");
            try
            {
                xref.ConnectionName = this.Name;
                if (_xrefs.ContainsKey(xref.Name))
                {
                    _xrefs[xref.Name] = xref;
                    r.Message = $"XrefTable {xref.Name} was updated.";
                }
                else
                {
                    _xrefs.Add(xref.Name, xref);
                }
            }
            catch (Exception ex)
            {
                SetErrorDetails(r, ex);
            }

            return r;
        }

        /// <summary>
        /// Removes the cross-reference table with the specified name.
        /// </summary>
        /// <param name="name">The name of the xref table to remove.</param>
        public void RemoveXref(string name)
        {
            if (_xrefs.ContainsKey(name))
            {
                _xrefs.Remove(name);
            }
        }

        /// <summary>
        /// Removes the specified cross-reference table.
        /// </summary>
        /// <param name="xrefTable">The xref table to remove.</param>
        public void RemoveXref(IXrefTable xrefTable)
        {
            if (_xrefs.ContainsKey(xrefTable.Name))
            {
                _xrefs.Remove(xrefTable.Name);
            }
        }

        /// <summary>
        /// Adds or updates a table in this schema definition.
        /// </summary>
        /// <param name="table">The table to add or update.</param>
        /// <returns>The result of the operation.</returns>
        public IDaoSchemaManagerResult AddTable(ITable table)
        {
            DaoSchemaManagerResult r = new DaoSchemaManagerResult($"Table {table.Name} was added.");
            try
            {
                table.ConnectionName = this.Name;
                if (this._tables.ContainsKey(table.Name))
                {
                    this._tables[table.Name] = table;
                    r.Message = $"Table {table.Name} was updated.";
                }
                else
                {
                    this._tables.Add(table.Name, table);
                }
            }
            catch (Exception ex)
            {
                SetErrorDetails(r, ex);
            }

            return r;
        }

        /// <summary>
        /// Adds or updates a foreign key in this schema definition.
        /// </summary>
        /// <param name="fk">The foreign key column to add or update.</param>
        /// <returns>The result of the operation.</returns>
        public IDaoSchemaManagerResult AddForeignKey(IForeignKeyColumn fk)
        {
            DaoSchemaManagerResult r = new DaoSchemaManagerResult($"ForeignKey {fk.ReferenceName} was added.");
            try
            {
                if (!this._foreignKeys.Contains(fk))
                {
                    this._foreignKeys.Add(fk);
                }
                else
                {
                    IForeignKeyColumn existing = (from efk in this._foreignKeys
                                                 where efk.Equals(fk)
                                                 select efk).FirstOrDefault();

                    existing.AllowNull = fk.AllowNull;
                    existing.DbDataType = fk.DbDataType;
                    existing.Key = fk.Key;
                    existing.MaxLength = fk.MaxLength;
                    existing.Name = fk.Name;
                    existing.ReferencedKey = fk.ReferencedKey;
                    existing.ReferencedTable = fk.ReferencedTable;
                    existing.ReferenceName = fk.ReferenceName;
                    existing.TableName = fk.TableName;                    
                }
            }
            catch (Exception ex)
            {
                SetErrorDetails(r, ex);
            }

            return r;
        }
        
        private void SetErrorDetails(IDaoSchemaManagerResult r, Exception ex)
        {
            this.LastException = ex;
            r.Message = ex.Message;
            r.Success = false;
            r.StackTrace = ex.StackTrace;
        }
        
        /// <summary>
        /// The most recent exception that occurred after trying to add a table
        /// or a foreign key
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        [XmlIgnore]
        public Exception LastException
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Loads a SchemaDefinition from the specified file, the file
        /// is created if it doesn't exist.
        /// </summary>
        /// <param name="schemaFile"></param>
        /// <returns></returns>
        public static DaoSchemaDefinition Load(string schemaFile)
        {
            DaoSchemaDefinition schema = new DaoSchemaDefinition {File = schemaFile};
            if (System.IO.File.Exists(schemaFile)) 
			{
	            schema = schemaFile.FromJsonFile<DaoSchemaDefinition>();
            }
            else
            {
                Save(schema);
            }
            return schema;
        }

        /// <summary>
        /// Serializes the current SchemaDefinition as json to the
        /// file specified in the File property
        /// </summary>
        public void Save()
        {
            Save(this);
        }

        /// <summary>
        /// Serializes the current SchemaDefinition as json to the
        /// specified filePath
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            this.File = filePath;
            Save(this);
        }

        /// <summary>
        /// Merges another schema definition into this one by adding all of its tables, foreign keys, and cross-reference tables.
        /// </summary>
        /// <param name="schemaDefinition">The schema definition to merge into this one.</param>
        /// <returns>This schema definition after merging.</returns>
        public IDaoSchemaDefinition CombineWith(IDaoSchemaDefinition schemaDefinition)
        {
            foreach (Table table in schemaDefinition.Tables)
            {
                AddTable(table);
            }

            foreach(IForeignKeyColumn foreignKey in schemaDefinition.ForeignKeys)
            {
                AddForeignKey(foreignKey);
            }
            
            foreach (XrefTable xref in schemaDefinition.Xrefs)
            {
                AddXref(xref);
            }

            return this;
        }
        
        static readonly object _saveLock = new object();
        private static void Save(DaoSchemaDefinition schema)
        {
            lock (_saveLock)
            {
                schema.ToJsonFile(schema.File);
            }
        }
    }
}
