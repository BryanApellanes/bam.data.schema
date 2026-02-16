/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines the contract for extracting a DAO schema definition from a data source.
    /// </summary>
    public interface IDaoSchemaExtractor
    {
        /// <summary>
        /// Gets or sets the mapping between database names and C# class/property names.
        /// </summary>
        SchemaNameMap NameMap { get; set; }

        /// <summary>
        /// Extracts a <see cref="DaoSchemaDefinition"/> from the data source.
        /// </summary>
        /// <returns>The extracted schema definition.</returns>
        DaoSchemaDefinition Extract();
    }
}
