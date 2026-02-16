using Bam.Data.Repositories;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines the contract for providing temporary file paths used during schema generation.
    /// </summary>
    public interface ISchemaTempPathProvider
    {
        /// <summary>
        /// Gets the temporary path for schema generation output based on both the schema definition and type schema.
        /// </summary>
        /// <param name="schemaDefinition">The DAO schema definition.</param>
        /// <param name="typeSchema">The type schema describing CLR type relationships.</param>
        /// <returns>A file system path for temporary schema output.</returns>
        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition, ITypeSchema typeSchema);

        /// <summary>
        /// Gets the temporary path for schema generation output based on the schema definition.
        /// </summary>
        /// <param name="schemaDefinition">The DAO schema definition.</param>
        /// <returns>A file system path for temporary schema output.</returns>
        string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition);
    }
}
