/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bam.Net;
using Bam.Net.Data;
using System.Web;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Linq.Expressions;
//using Bam.Net.Javascript;
using System.Threading;
using Bam.Net.Configuration;
using Bam.Net.Javascript;

namespace Bam.Net.Data.Schema
{
    [Proxy("schemaManager")]
    public partial class SchemaManager : IHasSchemaTempPathProvider
    {
        public SchemaManager(bool autoSave = true)
        {
            PreColumnAugmentations = new List<SchemaManagerAugmentation>();
            PostColumnAugmentations = new List<SchemaManagerAugmentation>();
            SchemaTempPathProvider = sd => Path.Combine(RuntimeSettings.ProcessDataFolder, "Schemas");
            AutoSave = autoSave;
        }

        public SchemaManager(string schemaFilePath)
            : this()
        {
            this.ManageSchema(schemaFilePath);
        }

        public SchemaManager(FileInfo schemaFile)
            : this(schemaFile.FullName)
        { }

        public SchemaManager(SchemaDefinition schema)
            : this()
        {
            this.ManageSchema(schema);
        }

        public Func<SchemaDefinition, string> SchemaTempPathProvider { get; set; }  

        public bool AutoSave { get; set; }

        SchemaDefinition _currentSchema;
        readonly object _currentSchemaLock = new object();
        public SchemaDefinition CurrentSchema
        {
            get
            {
                return _currentSchemaLock.DoubleCheckLock<SchemaDefinition>(ref _currentSchema, () => LoadSchema($"Default_{DateTime.Now.ToJulianDate().ToString()}"));
            }

            set => _currentSchema = value;
        }

        public void ManageSchema(string schemaFile)
        {
            SchemaDefinition schemaDefinition = schemaFile.FromJsonFile<SchemaDefinition>();
            if (schemaDefinition == null)
            {
                schemaDefinition = new SchemaDefinition();
                schemaDefinition.ToJsonFile(schemaFile);
            }

            schemaDefinition.File = schemaFile;
            ManageSchema(schemaDefinition);
        }

        public void ManageSchema(SchemaDefinition schema)
        {
            CurrentSchema = schema;
        }

        /// <summary>
        /// Loads the specified schema if it exists, saving it otherwise, and sets it as Current
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public SchemaDefinition SetSchema(string schemaName, bool useExisting = true)
        {
            string filePath = SchemaNameToFilePath(schemaName);
            lock (FileLock.Named(filePath))
            {
                if (!useExisting && File.Exists(filePath))
                {
                    if (BackupExisting)
                    {
                        string backUpPath = SchemaNameToFilePath("{0}_{1}_{2}"._Format(schemaName, DateTime.UtcNow.ToJulianDate(), 4.RandomLetters()));
                        File.Move(filePath, backUpPath);
                    }
                    else
                    {
                        File.Delete(filePath);
                    }
                }
                SchemaDefinition schema = LoadSchema(schemaName);
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
        public SchemaDefinition SetNewSchema(string schemaName)
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

        public SchemaDefinition GetCurrentSchema()
        {
            return CurrentSchema;
        }

        public ISchemaManagerResult AddTable(string tableName, string className = null)
        {
            try
            {
                SchemaDefinition schema = CurrentSchema;
                Table t = new Table();
                t.ClassName = className ?? tableName;
                t.Name = tableName;
                ISchemaManagerResult managerResult = schema.AddTable(t);
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

        public ISchemaManagerResult AddXref(string left, string right)
        {
            try
            {
                SchemaDefinition schema = CurrentSchema;
                XrefTable x = new XrefTable(left, right);
                ISchemaManagerResult managerResult = schema.AddXref(x);
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
        public ISchemaManagerResult SetColumnPropertyName(string tableName, string columnName, string propertyName)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.SetPropertyName(columnName, propertyName);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new SchemaManagerResult("column property name set");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public ISchemaManagerResult SetTableClassName(string tableName, string className)
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
                return new SchemaManagerResult("class name set");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }
        
        public ISchemaManagerResult AddColumn(string tableName, string columnName, DataTypes dataType = DataTypes.String)
        {
            return AddColumn(tableName, new Column(columnName, dataType));
        }

        /// <summary>
        /// Add the specified column to the specified table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public ISchemaManagerResult AddColumn(string tableName, Column column)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.AddColumn(column);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new SchemaManagerResult("column added");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public ISchemaManagerResult RemoveColumn(string tableName, string columnName)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.RemoveColumn(columnName);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new SchemaManagerResult("column removed");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public ISchemaManagerResult SetKeyColumn(string tableName, string columnName)
        {
            try
            {
                ITable table = CurrentSchema.GetTable(tableName);
                table.SetKeyColumn(columnName);
                if (AutoSave)
                {
                    CurrentSchema.Save();
                }
                return new SchemaManagerResult("Key column set");
            }
            catch (Exception ex)
            {
                return GetErrorResult(ex);
            }
        }

        public ISchemaManagerResult SetForeignKey(string targetTable, string referencingTable, string referencingColumn, string referencedKey = null, INameFormatter nameFormatter = null)
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

        protected virtual SchemaManagerResult SetForeignKey(ITable table, ITable target, IForeignKeyColumn fk)
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
            return new SchemaManagerResult("ForeignKeyColumn set");
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

        public SchemaManagerResult GenerateDaoAssembly(FileInfo databaseDotJs, bool compile = false, bool keepSource = false, string genTo = "./tmp", string partialsDir = null)
        {
            string result = databaseDotJs.JsonFromJsLiteralFile("database");
            return GenerateDaoAssembly(result, compile ? new DirectoryInfo(BinDir) : null, keepSource, genTo, partialsDir);
        }

        public SchemaManagerResult GenerateDaoAssembly(FileInfo databaseDotJs, DirectoryInfo compileTo, bool keepSource = false, string genTo = "./tmp", string partialsDir = null)
        {
            string result = databaseDotJs.JsonFromJsLiteralFile("database");

            return GenerateDaoAssembly(result, compileTo, keepSource, genTo, partialsDir);
        }

        public SchemaManagerResult GenerateDaoAssembly(FileInfo databaseDotJs, DirectoryInfo compileTo, DirectoryInfo temp, DirectoryInfo partialsDir)
        {
            string databaseSchemaJson = databaseDotJs.JsonFromJsLiteralFile("database");
            return GenerateDaoAssembly(databaseSchemaJson, compileTo, false, temp.FullName, partialsDir.FullName);
        }

        public SchemaManagerResult GenerateDaoAssembly(string simpleSchemaJson, DirectoryInfo compileTo, DirectoryInfo temp)
        {
            return GenerateDaoAssembly(simpleSchemaJson, compileTo, false, temp.FullName);
        }

        /// <summary>
        /// Generate 
        /// </summary>
        /// <param name="simpleSchemaJson"></param>
        /// <returns></returns>
        public SchemaManagerResult GenerateDaoAssembly(string simpleSchemaJson, DirectoryInfo compileTo = null, bool keepSource = false, string tempDir = "./tmp", string partialsDir = null)
        {
            try
            {
                bool compile = compileTo != null;
                SchemaManagerResult managerResult = new SchemaManagerResult("Generation completed");
                dynamic rehydrated = JsonConvert.DeserializeObject<dynamic>(simpleSchemaJson);
                if (rehydrated["nameSpace"] == null)// || rehydrated["schemaName"] == null)
                {
                    managerResult.ExceptionMessage = "Please specify nameSpace";
                    managerResult.Message = string.Empty;
                    managerResult.Success = false;
                }
                else if (rehydrated["schemaName"] == null)
                {
                    managerResult.ExceptionMessage = "Please specify schemaName";
                    managerResult.Message = string.Empty;
                    managerResult.Success = false;
                }
                else
                {
                    string nameSpace = (string)rehydrated["nameSpace"];
                    string schemaName = (string)rehydrated["schemaName"];
                    managerResult.Namespace = nameSpace;
                    managerResult.SchemaName = schemaName;
                    List<dynamic> foreignKeys = new List<dynamic>();

                    SetSchema(schemaName, false);

                    ProcessTables(rehydrated, foreignKeys);
                    ProcessXrefs(rehydrated, foreignKeys);

                    foreach (dynamic fk in foreignKeys)
                    {
                        AddColumn(fk.ForeignKeyTable, new Column(fk.ReferencingColumn, DataTypes.ULong));
                        SetForeignKey(fk.PrimaryTable, fk.ForeignKeyTable, fk.ReferencingColumn);
                    }

                    DirectoryInfo daoDir = new DirectoryInfo(tempDir);
                    if (!daoDir.Exists)
                    {
                        daoDir.Create();
                    }

                    DaoGenerator generator = GetDaoGenerator(compileTo, keepSource, partialsDir, compile, managerResult, nameSpace, daoDir);
                    generator.Generate(CurrentSchema, daoDir.FullName, partialsDir);
                    managerResult.DaoAssembly = generator.DaoAssemblyFile;
                }
                return managerResult;
            }
            catch (Exception ex)
            {
                SchemaManagerResult r = new SchemaManagerResult(ex.Message)
                {
                    StackTrace = ex.StackTrace ?? "", Success = false
                };
                return r;
            }
        }

        /// <summary>
        /// Augmentations that are executed prior to adding columns and 
        /// foreign keys
        /// </summary>
        public List<SchemaManagerAugmentation> PreColumnAugmentations
        {
            get;
            set;
        }

        /// <summary>
        /// Augmentations that are executed after adding columns 
        /// and foreign keys
        /// </summary>
        public List<SchemaManagerAugmentation> PostColumnAugmentations
        {
            get;
            set;
        }

        public string BinDir => Path.Combine(RootDir, "bin");

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


        /// <summary>
        /// Gets the most recent set of exceptions that occurred during an attempted
        /// Generate -> Compile
        /// </summary>
        public CompilerErrorCollection CompilerErrors
        {
            get;
            private set;
        }

        public CompilerError[] GetErrors()
        {
            if (CompilerErrors == null)
            {
                return new CompilerError[] { };
            }

            CompilerError[] results = new CompilerError[CompilerErrors.Count];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = CompilerErrors[i];
            }

            return results;
        }

        private void ProcessTables(dynamic rehydrated, List<dynamic> foreignKeys)
        {
            foreach (dynamic table in rehydrated["tables"])
            {
                string tableName = (string)table["name"];
                Args.ThrowIfNullOrEmpty(tableName, "Table.name");
                this.AddTable(tableName);

                ExecutePreColumnAugmentations(tableName);

                AddColumns(table, tableName);

                AddForeignKeys(foreignKeys, table, tableName);

                ExecutePostColumnAugmentations(tableName);
            }
        }

        internal void ExecutePostColumnAugmentations(string tableName)
        {
            ExecutePostColumnAugmentations(tableName, this);
        }
        
        private void ExecutePostColumnAugmentations(string tableName, SchemaManager manager)
        {
            foreach (SchemaManagerAugmentation augmentation in PostColumnAugmentations)
            {
                augmentation.Execute(tableName, manager);
            }
        }
        
        internal void ExecutePreColumnAugmentations(string tableName)
        {
            ExecutePreColumnAugmentations(tableName, this);
        }
        
        protected static void ExecutePreColumnAugmentations(string tableName, SchemaManager manager)
        {
            foreach (SchemaManagerAugmentation augmentation in manager.PreColumnAugmentations)
            {
                augmentation.Execute(tableName, manager);
            }
        }

        private static void AddForeignKeys(List<dynamic> foreignKeys, dynamic table, string tableName)
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

        private void AddColumns(dynamic table, string tableName)
        {
            AddColumns(this, table, tableName);
        }

        private static void AddColumns(SchemaManager manager, dynamic table, string tableName)
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

        private void ProcessXrefs(dynamic rehydrated, List<dynamic> foreignKeys)
        {
            ProcessXrefs(this, rehydrated, foreignKeys);
        }

        protected static void ProcessXrefs(SchemaManager manager, dynamic rehydrated, List<dynamic> foreignKeys)
        {
            if (rehydrated["xrefs"] != null)
            {
                foreach (dynamic xref in rehydrated["xrefs"])
                {
                    string leftTableName = (string)xref[0];
                    string rightTableName = (string)xref[1];

                    Args.ThrowIfNullOrEmpty(leftTableName, "xref[0]");
                    Args.ThrowIfNullOrEmpty(rightTableName, "xref[1]");

                    SetXref(manager, foreignKeys, leftTableName, rightTableName);
                }

            }
        }
        public void SetXref(List<dynamic> foreignKeys, string leftTableName, string rightTableName)
        {
            SetXref(this, foreignKeys, leftTableName, rightTableName);
        }

        private static void SetXref(SchemaManager manager, List<dynamic> foreignKeys, string leftTableName, string rightTableName)
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

        private FileInfo Compile(DirectoryInfo dir, DaoGenerator generator, string nameSpace, DirectoryInfo copyTo)
        {
            return Compile(new DirectoryInfo[] { dir }, generator, nameSpace, copyTo);
        }

        private FileInfo Compile(DirectoryInfo[] dirs, DaoGenerator generator, string nameSpace, DirectoryInfo copyTo)
        {
            string[] referenceAssemblies = DaoGenerator.DefaultReferenceAssemblies.ToArray();
            for (int i = 0; i < referenceAssemblies.Length; i++)
            {
                string assembly = referenceAssemblies[i];
                string binPath = Path.Combine(BinDir, assembly);

                referenceAssemblies[i] = File.Exists(binPath) ? binPath : assembly;
            }

            CompilerResults results = AdHocCSharpCompiler.CompileDirectories(dirs, $"{nameSpace}.dll", referenceAssemblies, false);
            if (results.Errors.Count > 0)
            {
                CompilerErrors = results.Errors;
                return null;
            }
            else
            {
                CompilerErrors = null;
                DirectoryInfo bin = new DirectoryInfo(BinDir);
                if (!bin.Exists)
                {
                    bin.Create();
                }

                FileInfo dll = new FileInfo(results.CompiledAssembly.CodeBase.Replace("file:///", ""));

                string binFile = Path.Combine(bin.FullName, dll.Name);
                string copy = Path.Combine(copyTo.FullName, dll.Name);
                if (File.Exists(binFile))
                {
                    BackupFile(binFile);
                }
                dll.CopyTo(binFile, true);
                if (!binFile.ToLowerInvariant().Equals(copy.ToLowerInvariant()))
                {
                    if (File.Exists(copy))
                    {
                        BackupFile(copy);
                    }

                    dll.CopyTo(copy);
                }

                return new FileInfo(copy);
            }
        }

        private static void BackupFile(string fileName)
        {
            FileInfo binFileInfo = new FileInfo(fileName);
            FileInfo backupFile = new FileInfo(Path.Combine(
                        binFileInfo.Directory.FullName,
                        "backup",
                        $"{Path.GetFileNameWithoutExtension(fileName)}_{"".RandomLetters(4)}_{DateTime.Now.ToJulianDate().ToString()}.dll"));

            if (!backupFile.Directory.Exists)
            {
                backupFile.Directory.Create();
            }
            binFileInfo.MoveTo(backupFile.FullName);
        }


        private SchemaDefinition LoadSchema(string schemaName)
        {
            string schemaFile = SchemaNameToFilePath(schemaName);
            SchemaDefinition schema = SchemaDefinition.Load(schemaFile);
            schema.Name = schemaName;
            return schema;
        }


        private static SchemaManagerResult GetErrorResult(Exception ex)
        {
            SchemaManagerResult managerResult = new SchemaManagerResult(ex.Message);
            managerResult.ExceptionMessage = ex.Message;
            managerResult.Success = false;
#if DEBUG
            managerResult.StackTrace = ex.StackTrace;
#endif
            return managerResult;
        }

        private DaoGenerator GetDaoGenerator(DirectoryInfo compileTo, bool keepSource, string partialsDir, bool compile, SchemaManagerResult managerResult, string nameSpace, DirectoryInfo daoDir)
        {
            DaoGenerator generator = new DaoGenerator(nameSpace);
            if (compile)
            {
                if (!compileTo.Exists)
                {
                    compileTo.Create();
                }

                generator.GenerateComplete += (gen, s) =>
                {
                    List<DirectoryInfo> daoDirs = new List<DirectoryInfo> {daoDir};
                    if (!string.IsNullOrEmpty(partialsDir))
                    {
                        daoDirs.Add(new DirectoryInfo(partialsDir));
                    }

                    gen.DaoAssemblyFile = Compile(daoDirs.ToArray(), gen, nameSpace, compileTo);

                    if (CompilerErrors != null)
                    {
                        managerResult.Success = false;
                        managerResult.Message = string.Empty;
                        foreach (CompilerError err in GetErrors())
                        {
                            managerResult.Message = $"{managerResult.Message}\r\nFile=>{err.FileName}\r\n{err.ErrorNumber}:::Line {err.Line}, Column {err.Column}::{err.ErrorText}";
                        }
                    }
                    else
                    {
                        managerResult.Message = $"{managerResult.Message}\r\nDao Compiled";
                        managerResult.Success = true;
                    }

                    if (!keepSource)
                    {
                        daoDir.Delete(true);
                        daoDir.Refresh();
                        if (daoDir.Exists)
                        {
                            daoDir.Delete();
                        }
                    }
                };
            }
            return generator;
        }
        
        private string SchemaNameToFilePath(string schemaName)
        {
            string schemaFile = Path.Combine(SchemaTempPathProvider(new SchemaDefinition { Name = schemaName }), "{0}.json"._Format(schemaName));
            return schemaFile;
        }
    }
}
