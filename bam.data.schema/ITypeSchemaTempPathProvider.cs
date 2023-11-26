using Bam.Net.Data.Repositories;
using Bam.Net.Data.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Data.Schema
{
    public interface ITypeSchemaTempPathProvider
    {
        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition, ITypeSchema typeSchema);

        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition);
    }
}
