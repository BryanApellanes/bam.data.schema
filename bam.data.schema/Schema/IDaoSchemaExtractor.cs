/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public interface IDaoSchemaExtractor
    {
        SchemaNameMap NameMap { get; set; }
        DaoSchemaDefinition Extract();
    }
}
