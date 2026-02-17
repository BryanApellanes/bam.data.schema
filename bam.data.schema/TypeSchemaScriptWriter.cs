using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Generates and executes SQL schema scripts from CLR types using a <see cref="TypeInheritanceSchemaProvider"/>.
    /// </summary>
    public class TypeSchemaScriptWriter
    {
        /// <summary>
        /// Gets or sets the result of the most recent schema definition creation.
        /// </summary>
        public DaoSchemaDefinitionCreateResult LastSchemaDefinitionCreateResult { get; set; } = null!;

        /// <summary>
        /// Generates and executes the SQL schema script for the specified types against the given database.
        /// </summary>
        /// <param name="database">The database to execute the schema script against.</param>
        /// <param name="types">The CLR types to generate a schema for.</param>
        public void CommitSchema(Database database, IEnumerable<Type> types)
        {
            database.ExecuteSql(WriteSchemaScript(database, types));
        }
        /// <summary>
        /// Generates and executes the SQL schema script for the specified types against the given database.
        /// </summary>
        /// <param name="database">The database to execute the schema script against.</param>
        /// <param name="types">The CLR types to generate a schema for.</param>
        public void CommitSchema(IDatabase database, params Type[] types)
        {
            database.ExecuteSql(WriteSchemaScript(database, types));
        }

        /// <summary>
        /// Generates the SQL schema script for the specified types without executing it.
        /// </summary>
        /// <param name="database">The database whose dialect to use for the script.</param>
        /// <param name="types">The CLR types to generate a schema for.</param>
        /// <returns>The SQL string builder containing the generated schema script.</returns>
        public ISqlStringBuilder WriteSchemaScript(IDatabase database, IEnumerable<Type> types)
        {
            return WriteSchemaScript(database, types.ToArray());
        }

        /// <summary>
        /// Write and return the sql schema script using a TypeInheritanceSchemaGenerator
        /// </summary>
        /// <param name="database"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public ISqlStringBuilder WriteSchemaScript(IDatabase database, params Type[] types)
        {
            TypeInheritanceSchemaProvider schemaGenerator = new TypeInheritanceSchemaProvider {Types = types};
            return WriteSchemaScript(database, schemaGenerator);
        }

        /// <summary>
        /// Generates the SQL schema script using the specified schema provider and optional schema manager.
        /// </summary>
        /// <param name="database">The database whose dialect to use for the script.</param>
        /// <param name="typeSchemaGenerator">The schema provider to use for generating the DAO schema definition.</param>
        /// <param name="schemaManager">An optional schema manager; if null, a non-auto-saving manager is created.</param>
        /// <returns>The SQL string builder containing the generated schema script.</returns>
        public SqlStringBuilder WriteSchemaScript(IDatabase database, SchemaProvider typeSchemaGenerator, DaoSchemaManager? schemaManager = null)
        {
            schemaManager = schemaManager ?? new DaoSchemaManager { AutoSave = false };
            typeSchemaGenerator.SchemaManager = schemaManager;
            LastSchemaDefinitionCreateResult = typeSchemaGenerator.CreateDaoSchemaDefinition();
            return WriteSchemaScript(database, LastSchemaDefinitionCreateResult);
        }

        /// <summary>
        /// Generates the SQL schema script from a previously created schema definition result.
        /// </summary>
        /// <param name="database">The database whose dialect to use for the script.</param>
        /// <param name="schemaDefinitionCreateResult">The schema definition create result containing tables and foreign keys.</param>
        /// <returns>The SQL string builder containing the generated schema script.</returns>
        public SqlStringBuilder WriteSchemaScript(IDatabase database, DaoSchemaDefinitionCreateResult schemaDefinitionCreateResult)
        {
            IDaoSchemaDefinition schemaDefinition = schemaDefinitionCreateResult.DaoSchemaDefinition;
            SchemaWriter writer = database.GetService<SchemaWriter>();
            IEnumerable<ForeignKeyAttribute> fks = GetForeignKeyAttributes(schemaDefinition);
            
            schemaDefinition.Tables.Each(table =>
            {
                string columnDefinitions = GetColumnDefinitions(table, writer);
                writer.WriteCreateTable(table.Name, columnDefinitions, fks.Where(fk=> fk.Table.Equals(table.Name)).ToArray());
                writer.Go();
            });

            schemaDefinition.ForeignKeys.Each(fk =>
            {
                writer.WriteAddForeignKey(fk.TableName, fk.ReferenceName, fk.Name, fk.ReferencedTable, fk.ReferencedKey);
                writer.Go();
            });

            return writer;
        }

        private static string GetColumnDefinitions(ITable table, SchemaWriter writer)
        {
            List<string> columnSegments = new List<string>();
            table.Columns.Each(column =>
            {
                if (column.Key)
                {
                    KeyColumnAttribute keyAttr = GetColumnAttribute<KeyColumnAttribute>(column);
                    columnSegments.Add(writer.GetKeyColumnDefinition(keyAttr));
                }
                else
                {
                    ColumnAttribute colAttr = GetColumnAttribute<ColumnAttribute>(column);
                    columnSegments.Add(writer.GetColumnDefinition(colAttr));
                }
            });
            return string.Join(", ", columnSegments.ToArray());
        }

        private static T GetColumnAttribute<T>(IColumn column) where T : ColumnAttribute, new()
        {
            return new T { Name = column.Name, AllowNull = column.AllowNull, DbDataType = column.DbDataType, MaxLength = column.MaxLength, Table = column.TableName };
        }
        
        private static IEnumerable<ForeignKeyAttribute> GetForeignKeyAttributes(IDaoSchemaDefinition schema)
        {
            foreach(ForeignKeyColumn fk in schema.ForeignKeys)
            {
                yield return new ForeignKeyAttribute { Table = fk.TableName, Name = fk.Name, ReferencedTable = fk.ReferencedTable, ReferencedKey = fk.ReferencedKey };
            }
        }
    }
}
