namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines the contract for resolving output streams for each type of generated DAO source file.
    /// </summary>
    public interface IDaoTargetStreamResolver
    {
        /// <summary>
        /// Gets the output stream for the context class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="schema">The schema definition.</param>
        /// <returns>The output stream to write the context class to.</returns>
        Stream GetTargetContextStream(Func<string, Stream> targetResolver, string rootDirectory, IDaoSchemaDefinition schema);

        /// <summary>
        /// Gets the output stream for a DAO class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the DAO class for.</param>
        /// <returns>The output stream to write the DAO class to.</returns>
        Stream GetTargetClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        /// <summary>
        /// Gets the output stream for a query class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the query class for.</param>
        /// <returns>The output stream to write the query class to.</returns>
        Stream GetTargetQueryClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        /// <summary>
        /// Gets the output stream for a paged query class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the paged query class for.</param>
        /// <returns>The output stream to write the paged query class to.</returns>
        Stream GetTargetPagedQueryClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        /// <summary>
        /// Gets the output stream for a Qi (query interface) class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the Qi class for.</param>
        /// <returns>The output stream to write the Qi class to.</returns>
        Stream GetTargetQiClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        /// <summary>
        /// Gets the output stream for a collection class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the collection class for.</param>
        /// <returns>The output stream to write the collection class to.</returns>
        Stream GetTargetCollectionStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        /// <summary>
        /// Gets the output stream for a columns class.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the columns class for.</param>
        /// <returns>The output stream to write the columns class to.</returns>
        Stream GetTargetColumnsClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        /// <summary>
        /// Gets the output stream for a partial class stub.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="rootDirectory">The root directory for output files.</param>
        /// <param name="table">The table to generate the partial class for.</param>
        /// <returns>The output stream to write the partial class to.</returns>
        Stream GetTargetPartialClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
    }
}
