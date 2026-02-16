namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines a type that has a schema temporary path provider function.
    /// </summary>
    public interface IHasSchemaTempPathProvider
    {
        /// <summary>
        /// Gets or sets a function that provides a temporary file path based on a schema definition.
        /// </summary>
        Func<IDaoSchemaDefinition, string> SchemaTempPathProvider { get; set; }
    }
}
