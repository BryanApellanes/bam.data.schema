using Bam.Data.Repositories;

namespace Bam.Data.Schema
{
    /// <summary>
    /// A class used to generate TypeSchemas.  A TypeSchema is 
    /// a class that provides database schema like relationship
    /// descriptors for CLR types. This implementation accounts
    /// for inheritance relationships in the CLR types and
    /// breaks sub types into separate tables.
    /// </summary>
    public class TypeInheritanceSchemaProvider : SchemaProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TypeInheritanceSchemaProvider"/> with the specified table name and temp path providers.
        /// </summary>
        /// <param name="tableNameProvider">The provider to use for determining table names from CLR types; defaults to <see cref="EchoTypeTableNameProvider"/> if null.</param>
        /// <param name="schemaTempPathProvider">The provider for temporary file paths during schema generation; defaults to <see cref="SchemaTempPathProvider"/> if null.</param>
        public TypeInheritanceSchemaProvider(ITypeTableNameProvider tableNameProvider = null, ISchemaTempPathProvider schemaTempPathProvider = null)
            : base(tableNameProvider, schemaTempPathProvider)
        {
        }

        protected override void AddSchemaTables(TypeSchema typeSchema, DaoSchemaManager schemaManager, ITypeTableNameProvider tableNameProvider = null)
        {
            tableNameProvider = tableNameProvider ?? new EchoTypeTableNameProvider();
            foreach (Type topType in typeSchema.Tables)
            {
                TypeInheritanceDescriptor inheritance = new TypeInheritanceDescriptor(topType);
                Type inheritFrom = null;
                inheritance.Chain.BackwardsEach(typeTable =>
                {
                    string tableName = typeTable.GetTableName(tableNameProvider);
                    schemaManager.AddTable(tableName);
                    schemaManager.ExecutePreColumnAugmentations(tableName);
                    typeTable.PropertyColumns.Each(pc =>
                    {
                        AddPropertyColumn(schemaManager, typeSchema.DefaultDataTypeBehavior, tableName, pc.PropertyInfo);
                    });
                    schemaManager.ExecutePostColumnAugmentations(tableName);
                    schemaManager.AddColumn(tableName, "Id", DataTypes.ULong);
                    if (inheritFrom != null)
                    {
                        schemaManager.SetForeignKey(tableNameProvider.GetTableName(inheritFrom), tableName, "Id", "Id");
                    }
                    else
                    {
                        schemaManager.SetKeyColumn(tableName, "Id");
                    }
                    inheritFrom = typeTable.Type;
                });
            }
        }
    }
}
