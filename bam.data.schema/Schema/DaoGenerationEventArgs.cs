namespace Bam.Data.Schema
{
    /// <summary>
    /// Event arguments for DAO generation events, containing the schema definition and the table being processed.
    /// </summary>
    public class DaoGenerationEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DaoGenerationEventArgs"/>.
        /// </summary>
        public DaoGenerationEventArgs()
        {

        }

        /// <summary>
        /// Gets or sets the schema definition being generated.
        /// </summary>
        public DaoSchemaDefinition SchemaDefinition { get; set; }

        /// <summary>
        /// Gets or sets the table currently being processed during generation.
        /// </summary>
        public Table Table { get; set; }
    }
}
