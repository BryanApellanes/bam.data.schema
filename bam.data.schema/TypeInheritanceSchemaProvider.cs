using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Bam.Net.Data.Schema;
using Bam.Net;
using Bam.Net.Data;
using Bam.Net.Data.Repositories;

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
