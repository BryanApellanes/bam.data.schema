using Bam.Data.Repositories;
using Bam.Data.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Data.Schema
{
    public interface ISchemaTempPathProvider
    {
        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition, ITypeSchema typeSchema);

        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition);
    }
}
