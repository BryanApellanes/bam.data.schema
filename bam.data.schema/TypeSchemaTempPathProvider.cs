using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Net.Data.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Data.Schema
{
    public class TypeSchemaTempPathProvider : ITypeSchemaTempPathProvider
    {
        public static implicit operator Func<IDaoSchemaDefinition, ITypeSchema, string>(TypeSchemaTempPathProvider typeSchemaTempPathProvider)
        {
            return typeSchemaTempPathProvider.GetSchemaTempPath;
        }

        public TypeSchemaTempPathProvider() { }

        private Func<IDaoSchemaDefinition, ITypeSchema, string>? impl;
        public TypeSchemaTempPathProvider(Func<IDaoSchemaDefinition, ITypeSchema, string>? impl)
        {
            this.impl = impl;
        }

        public string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition, ITypeSchema typeSchema)
        {
            if(impl != null)
            {
                return impl(schemaDefinition, typeSchema);
            }
            return GetSchemaTempPath(schemaDefinition); // This implementation ignores typeSchema
        }

        public string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition)
        {
            if (impl != null)
            {
                return impl(schemaDefinition, null);
            }
            return Path.Combine(RuntimeSettings.ProcessDataFolder, $"DaoTemp_{schemaDefinition.Name}");
        }
    }
}
