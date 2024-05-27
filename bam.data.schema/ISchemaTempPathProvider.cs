using Bam.Data.Repositories;
using Bam.Data.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
