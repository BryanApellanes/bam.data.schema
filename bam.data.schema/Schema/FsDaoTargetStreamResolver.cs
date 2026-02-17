namespace Bam.Data.Schema
{
    /// <summary>
    /// Resolves output streams for generated DAO source files by writing to the file system, or using a custom target resolver function if provided.
    /// </summary>
    public class FsDaoTargetStreamResolver: IDaoTargetStreamResolver
    {
        /// <inheritdoc />
        public Stream GetTargetContextStream(Func<string, Stream> targetResolver, string root, IDaoSchemaDefinition schema)
        {
            string parameterValue = $"{schema.Name}Context";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = table.ClassName;
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetQueryClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}Query";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetPagedQueryClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}PagedQuery";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetQiClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"Qi/{table.ClassName}";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetCollectionStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}Collection";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetColumnsClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}Columns";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        /// <inheritdoc />
        public Stream GetTargetPartialClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            if(targetResolver != null)
            {
                return targetResolver($"{table.Name}_Partial");
            }
            string path = Path.Combine(root, "Partials", $"{table.Name}.cs");
            FileInfo f = new FileInfo(path);
            if (!f.Directory!.Exists)
            {
                f.Directory.Create();
            }
            return f.OpenWrite();            
        }

        /// <summary>
        /// Gets the output stream for the specified parameter value, using the target resolver if provided, otherwise creating a file in the root directory.
        /// </summary>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root directory for output files.</param>
        /// <param name="parameterValue">The file name stem for the output file.</param>
        /// <returns>The output stream.</returns>
        public Stream GetTargetStream(Func<string, Stream>? targetResolver, string root, string parameterValue)
        {
            Stream stream;
            if (targetResolver != null)
            {
                stream = targetResolver(parameterValue);
            }
            else
            {
                string path = Path.Combine(root, $"{parameterValue}.cs");
                FileInfo file = new FileInfo(path);
                if (file.Directory != null && file.Directory.Exists == false)
                {
                    file.Directory.Create();
                }
                stream = file.OpenWrite();
            }
            return stream;
        }
    }
}
