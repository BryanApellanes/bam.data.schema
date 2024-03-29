/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Net;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Newtonsoft.Json;
//using Bam.Net.Javascript;

namespace Bam.Net.Data.Schema
{
    [Proxy("schemaManager")]
    public class DaoSchemaManager : IHasSchemaTempPathProvider
    {
        public DaoSchemaManager(bool autoSave = true)
        {
            PreColumnAugmentations = new List<DaoSchemaManagerAugmentation>();
            PostColumnAugmentations = new List<DaoSchemaManagerAugmentation>();
            SchemaTempPathProvider = sd => Path.Combine(RuntimeSettings.ProcessDataFolder, "Schemas");
            AutoSave = autoSave;
        }

        public DaoSchemaManager(string schemaFilePath)
            : this()
        {
            this.ManageSchema(schemaFilePath);
        }

        public DaoSchemaManager(IDaoSchemaDefinition schema)
            : this()
        {
            this.ManageSchema(schema);
        }


        public Func<IDaoSchemaDefinition, string> SchemaTempPathProvider { get; set; }  

        public bool AutoSave { get; set; }

        IDaoSchemaDefinition _currentSchema;
        readonly object _currentSchemaLock = new object();
        public IDaoSchemaDefinition CurrentSchema
        {
            get
            {
                return _currentSchemaLock.DoubleCheckLock(ref _currentSchema, () => LoadSchema($"Default_{DateTime.Now.ToJulianDate()}"));
            }

            set => _currentSchema = value;
        }

        public void ManageSchema(string schemaFile)
        {
            DaoSchemaDefinition schemaDefinition = schemaFile.FromJsonFile<DaoSchemaDefinition>();
            if (schemaDefinition == null)
            {
                schemaDefinition = new DaoSchemaDefinition();
                schemaDefinition.ToJsonFile(schemaFile);
            }

            schemaDefinition.File = schemaFile;
            ManageSchema(schemaDefinition);
        }

        public void ManageSchema(IDaoSchemaDefinition schema)
        {
            CurrentSchema = schema;
        }

        /// <summary>
        /// Loads the specified schema if it exists, saving it otherwise, and sets it as Current
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public DaoSchemaDefinition SetSchema(string schemaName, bool useExisting = true)
        {
            string filePath = SchemaNameToFilePath(schemaName);
            lock (FileLock.Named(filePath))
            {
                if (!useExisting && File.Exists(filePath))
                {
                    if (BackupExisting)
                    {
                        string backUpPath = SchemaNameToFilePath("{0}_{1}_{2}".Format(schemaName, DateTime.UtcNow.ToJulianDate(), 4.RandomLetters()));
                        File.Move(filePath, backUpPath);
                    }
                    else
                    {
                        File.Delete(filePath);
                    }
                }
                DaoSchemaDefinition schema = LoadSchema(schemaName);
                CurrentSchema = schema;
                return schema;
            }
        }

        /// <summary>
        /// If true backup any existing schema file appending the Julian date plus 4 random characters to the 
        /// existing file name.  Otherwise the file will be deleted.
        /// </summary>
        public bool BackupExisting { get; set; }

        /// <summary>
        /// Calls SetSchema if the specified schema does not already
        /// exist.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public DaoSchemaDefinition SetNewSchema(string schemaName)
        {
            if (SchemaExists(schemaName))
            {
                throw new InvalidOperationException("The specified schema already exists");
            }

            return SetSchema(schemaName);
        }

        public ITable GetTable(string tableName)
        {
            return CurrentSchema.GetTable(tableName);
        }

        public IXrefTable GetXref(string tableName)
        {
            return CurrentSchema.GetXref(tableName);
        }

        public bool SchemaExists(string schemaName)
        {
            return File.Exists(SchemaNameToFilePath(schemaName));
        }

        public IDaoSchemaDefinition GetCurrentSchema()
        {
            return CurrentSchema;
        }

        public IDaoSchemaManagerResult AddTable(string tableName, string className = null)
        {
            try
            {
                IDaoSchemaDefinition schema = CurrentSchema;
                Table t = new Table();
                t.ClassName = className ?? tableName;
                t.Name = tableName;
                IDaoSchemaManagerResult managerResult = schema.AddTable(t);
                if (AutoSave)
                {
                    schema.Save();
                }
                return managerResult;
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public IDaoSchemaManagerResult AddXref(string left, string right)
        {
            try
            {
                IDaoSchemaDefinition schema = CurrentSchema;
                XrefTable x = new XrefTable(left, right);
                IDaoSchemaManagerResult managerResult = schema.AddXref(x);
                if (AutoSave)
                {
                    schema.Save();
                }
                return managerResult;
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        /// <summary>
        /// Used to specify a different property name to use
        /// on generated Dao instead of the column name
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IDaoSchemaManagerResult SetColumnPropertyName(string tableName, string columnName, string propertyName)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.SetPropertyName(columnName, propertyName);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new DaoSchemaManagerResult("column property name set");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public IDaoSchemaManagerResult SetTableClassName(string tableName, string className)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.ClassName = className;
                table.Columns.Each(col =>
                {
                    col.TableClassName = className;
                });
                SetForeignKeyClassNames();
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new DaoSchemaManagerResult("class name set");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }
        
        public IDaoSchemaManagerResult AddColumn(string tableName, string columnName, DataTypes dataType = DataTypes.String)
        {
            return AddColumn(tableName, new Column(columnName, dataType));
        }

        /// <summary>
        /// Add the specified column to the specified table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public IDaoSchemaManagerResult AddColumn(string tableName, Column column)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.AddColumn(column);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new DaoSchemaManagerResult("column added");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public IDaoSchemaManagerResult RemoveColumn(string tableName, string columnName)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.RemoveColumn(columnName);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new DaoSchemaManagerResult("column removed");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public IDaoSchemaManagerResult SetKeyColumn(string tableName, string columnName)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.SetKeyColumn(columnName);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new DaoSchemaManagerResult("Key column set");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public IDaoSchemaManagerResult SetForeignKey(string targetTable, string referencingTable, string referencingColumn, string referencedKey = null, INameFormatter nameFormatter = null)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(referencingTable);
                ITable target = CurrentSchema.GetTable(targetTable);
                IColumn col = table[referencingColumn];
                if (col.DataType == DataTypes.Int || col.DataType == DataTypes.UInt ||
                    col.DataType == DataTypes.Long || col.DataType == DataTypes.ULong)
                {
                    IForeignKeyColumn fk = new ForeignKeyColumn(col, targetTable)
                    {
                        ReferencedKey = referencedKey ?? (target.Key != null ? target.Key.Name : "Id"),
                        ReferencedTable = target.Name
                    };
                    if (nameFormatter != null)
                    {
                        fk.ReferencedClass = nameFormatter.FormatClassName(targetTable);
                        fk.ReferencingClass = nameFormatter.FormatClassName(referencingTable);
                        fk.TableClassName = nameFormatter.FormatClassName(fk.TableName);
                        fk.PropertyName = nameFormatter.FormatPropertyName(fk.TableName, fk.Name);
                    }
                    return SetForeignKey(table, target, fk);
                }
                else
                {
                    throw new InvalidOperationException("The specified column must be a number type");
                }
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        protected void SetForeignKeyClassNames()
        {
            CurrentSchema.Tables.Each(table =>
            {
                IForeignKeyColumn[] referencingKeys = GetReferencingForeignKeysForTable(table.Name);
                referencingKeys.Each(fk =>
                {
                    fk.ReferencedClass = table.ClassName;
                });
                IForeignKeyColumn[] fks = GetForeignKeysForTable(table.Name);
                fks.Each(fk =>
                {
                    fk.ReferencingClass = table.ClassName;
                    fk.TableClassName = table.ClassName;
                });
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
            });
        }

        protected virtual DaoSchemaManagerResult SetForeignKey(ITable table, ITable target, IForeignKeyColumn fk)
        {
            CurrentSchema.AddForeignKey(fk);
            table.RemoveColumn(fk.Name);
            table.AddColumn(fk);
            target.ReferencingForeignKeys = GetReferencingForeignKeysForTable(target.Name);
            table.ForeignKeys = GetForeignKeysForTable(table.Name);
            if (AutoSave)
            {
                CurrentSchema.Save();
            }
            return new DaoSchemaManagerResult("ForeignKeyColumn set");
        }

        protected void SetXrefs(XrefTable[] xrefs)
        {
            CurrentSchema.Xrefs = xrefs;
        }

        /// <summary>
        /// Get the ForeignKeyColumns where the specified table is the 
        /// referenced table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected IForeignKeyColumn[] GetReferencingForeignKeysForTable(string tableName)
        {
            string lowered = tableName.ToLowerInvariant();
            return (from f in CurrentSchema.ForeignKeys
                    where f.ReferencedTable.ToLowerInvariant().Equals(lowered)
                    select f).ToArray();
        }

        /// <summary>
        /// Get the ForeignKeyColumns defined on the specified table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected IForeignKeyColumn[] GetForeignKeysForTable(string tableName)
        {
            string lowered = tableName.ToLowerInvariant();
            return (from f in CurrentSchema.ForeignKeys
                    where f.TableName.ToLowerInvariant().Equals(lowered)
                    select f).ToArray();
        }

        readonly object _sync = new object();
        public void Save()
        {
            lock (_sync)
            {
                CurrentSchema.ToJsonFile(CurrentSchema.File);
            }
        }

        public void RemoveTable(string tableName)
        {
            CurrentSchema.RemoveTable(tableName);
        }

        
        /// <summary>
        /// Augmentations that are executed prior to adding columns and 
        /// foreign keys
        /// </summary>
        public List<DaoSchemaManagerAugmentation> PreColumnAugmentations
        {
            get;
            set;
        }

        /// <summary>
        /// Augmentations that are executed after adding columns 
        /// and foreign keys
        /// </summary>
        public List<DaoSchemaManagerAugmentation> PostColumnAugmentations
        {
            get;
            set;
        }


        /// <summary>
        /// Adds a cross reference table (xref) which creates a many
        /// to many relationship between the two tables specified
        /// </summary>
        /// <param name="leftTableName"></param>
        /// <param name="rightTableName"></param>
        public void SetXref(string leftTableName, string rightTableName)
        {
            string xrefTableName = $"{leftTableName}{rightTableName}";
            string leftColumnName = $"{leftTableName}Id";
            string rightColumnName = $"{rightTableName}Id";
            AddXref(leftTableName, rightTableName);
            AddTable(xrefTableName);
            AddColumn(xrefTableName, new Column("Id", DataTypes.ULong, false));
            SetKeyColumn(xrefTableName, "Id");
            AddColumn(xrefTableName, new Column("Uuid", DataTypes.String, false));
            AddColumn(xrefTableName, new Column(leftColumnName, DataTypes.ULong, false));
            AddColumn(xrefTableName, new Column(rightColumnName, DataTypes.ULong, false));
            SetForeignKey(leftTableName, xrefTableName, leftColumnName);
            SetForeignKey(rightTableName, xrefTableName, rightColumnName);
        }




        internal void ExecutePostColumnAugmentations(string tableName)
        {
            ExecutePostColumnAugmentations(tableName, this);
        }
        
        protected void ExecutePostColumnAugmentations(string tableName, DaoSchemaManager manager)
        {
            foreach (DaoSchemaManagerAugmentation augmentation in PostColumnAugmentations)
            {
                augmentation.Execute(tableName, manager);
            }
        }
        
        internal void ExecutePreColumnAugmentations(string tableName)
        {
            ExecutePreColumnAugmentations(tableName, this);
        }
        
        protected static void ExecutePreColumnAugmentations(string tableName, DaoSchemaManager manager)
        {
            foreach (DaoSchemaManagerAugmentation augmentation in manager.PreColumnAugmentations)
            {
                augmentation.Execute(tableName, manager);
            }
        }

        protected static void AddForeignKeys(List<dynamic> foreignKeys, dynamic table, string tableName)
        {
            if (table["fks"] != null)
            {
                foreach (dynamic fk in table["fks"])
                {
                    PropertyDescriptorCollection fkProperties = TypeDescriptor.GetProperties(fk);
                    foreach (PropertyDescriptor pd in fkProperties)
                    {
                        string referencingColumn = pd.Name;
                        string primaryTable = (string)pd.GetValue(fk);
                        string foreignKeyTable = tableName;
                        AddForeignKey(foreignKeys, primaryTable, foreignKeyTable, referencingColumn);
                    }
                }
            }
        }

        protected void AddColumns(dynamic table, string tableName)
        {
            AddColumns(this, table, tableName);
        }

        private static void AddColumns(DaoSchemaManager manager, dynamic table, string tableName)
        {
            if (table["cols"] != null)
            {
                foreach (dynamic column in table["cols"])
                {
                    PropertyDescriptorCollection columnProperties = TypeDescriptor.GetProperties(column);
                    bool allowNull = column["Null"] == null || (bool)column["Null"];
                    string maxLength = column["MaxLength"] == null ? "" : (string)column["MaxLength"];
                    foreach (PropertyDescriptor pd in columnProperties)
                    {
                        if (!pd.Name.Equals("Null") && !pd.Name.Equals("MaxLength"))
                        {
                            DataTypes type = (DataTypes)Enum.Parse(typeof(DataTypes), (string)pd.GetValue(column));
                            string name = pd.Name;
                            manager.AddColumn(tableName, new Column(name, type, allowNull, maxLength));
                        }
                    }
                }
            }
        }

        public void SetXref(List<dynamic> foreignKeys, string leftTableName, string rightTableName)
        {
            SetXref(this, foreignKeys, leftTableName, rightTableName);
        }

        protected static void SetXref(DaoSchemaManager manager, List<dynamic> foreignKeys, string leftTableName, string rightTableName)
        {
            string xrefTableName = $"{leftTableName}{rightTableName}";
            string leftColumnName = $"{leftTableName}Id";
            string rightColumnName = $"{rightTableName}Id";

            manager.AddXref(leftTableName, rightTableName);

            manager.AddTable(xrefTableName);
            manager.AddColumn(xrefTableName, new Column("Id", DataTypes.ULong, false));
            manager.SetKeyColumn(xrefTableName, "Id");
            manager.AddColumn(xrefTableName, new Column("Uuid", DataTypes.String, false));
            manager.AddColumn(xrefTableName, new Column(leftColumnName, DataTypes.ULong, false));
            manager.AddColumn(xrefTableName, new Column(rightColumnName, DataTypes.ULong, false));

            AddForeignKey(foreignKeys, leftTableName, xrefTableName, leftColumnName);
            AddForeignKey(foreignKeys, rightTableName, xrefTableName, rightColumnName);
        }

        private static void AddForeignKey(List<dynamic> foreignKeys, string primaryTable, string foreignKeyTable, string referencingColumnName)
        {
            foreignKeys.Add(new { PrimaryTable = primaryTable, ForeignKeyTable = foreignKeyTable, ReferencingColumn = referencingColumnName });
        }

        private DaoSchemaDefinition LoadSchema(string schemaName)
        {
            string schemaFile = SchemaNameToFilePath(schemaName);
            DaoSchemaDefinition schema = DaoSchemaDefinition.Load(schemaFile);
            schema.Name = schemaName;
            return schema;
        }


        private static DaoSchemaManagerResult GetErrorResult(Exception ex)
        {
            DaoSchemaManagerResult managerResult = new DaoSchemaManagerResult(ex.Message);
            managerResult.ExceptionMessage = ex.Message;
            managerResult.Success = false;
#if DEBUG
            managerResult.StackTrace = ex.StackTrace;
#endif
            return managerResult;
        }

        
        private string SchemaNameToFilePath(string schemaName)
        {
            string schemaFile = Path.Combine(SchemaTempPathProvider(new DaoSchemaDefinition { Name = schemaName }), "{0}.json".Format(schemaName));
            return schemaFile;
        }
    }
}
