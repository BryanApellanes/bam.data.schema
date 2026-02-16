namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines the contract for writing generated DAO source code files from a schema definition.
    /// </summary>
    public interface IDaoCodeWriter
    {
        /// <summary>
        /// Gets or sets the namespace for generated code.
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the resolver for determining output streams for generated files.
        /// </summary>
        IDaoTargetStreamResolver DaoTargetStreamResolver { get; set; }

        /// <summary>
        /// Writes the context class for the schema definition.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        void WriteContextClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root);

        /// <summary>
        /// Writes the DAO class for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the DAO class for.</param>
        void WriteDaoClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        /// <summary>
        /// Writes the query class for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the query class for.</param>
        void WriteQueryClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        /// <summary>
        /// Writes the paged query class for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the paged query class for.</param>
        void WritePagedQueryClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        /// <summary>
        /// Writes the Qi (query interface) class for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the Qi class for.</param>
        void WriteQiClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        /// <summary>
        /// Writes the collection class for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the collection class for.</param>
        void WriteCollectionClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        /// <summary>
        /// Writes the columns class for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the columns class for.</param>
        void WriteColumnsClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        /// <summary>
        /// Writes a partial class stub for the specified table.
        /// </summary>
        /// <param name="schema">The schema definition.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="table">The table to generate the partial class for.</param>
        void WritePartial(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
    }
}
