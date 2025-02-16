using Bam.Data.Repositories;

namespace Bam.Data.Schema
{
    public interface ISchemaTempPathProvider
    {
        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition, ITypeSchema typeSchema);

        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition);
    }
}
