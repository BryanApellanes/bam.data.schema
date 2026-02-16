using Bam.Data.Repositories;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Provides temporary file system paths for schema generation output, defaulting to a subdirectory of the process data folder.
    /// </summary>
    public class SchemaTempPathProvider : ISchemaTempPathProvider
    {
        /// <summary>
        /// Implicitly converts a <see cref="SchemaTempPathProvider"/> to a <see cref="Func{IDaoSchemaDefinition, ITypeSchema, String}"/> delegate.
        /// </summary>
        /// <param name="typeSchemaTempPathProvider">The provider to convert.</param>
        public static implicit operator Func<IDaoSchemaDefinition, ITypeSchema, string>(SchemaTempPathProvider typeSchemaTempPathProvider)
        {
            return typeSchemaTempPathProvider.GetSchemaTempPath;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SchemaTempPathProvider"/> with the default path logic.
        /// </summary>
        public SchemaTempPathProvider() { }

        private Func<IDaoSchemaDefinition, ITypeSchema?, string>? impl;

        /// <summary>
        /// Initializes a new instance of <see cref="SchemaTempPathProvider"/> with a custom path resolution function.
        /// </summary>
        /// <param name="impl">A function that takes a schema definition and optional type schema and returns a temporary path.</param>
        public SchemaTempPathProvider(Func<IDaoSchemaDefinition, ITypeSchema?, string>? impl)
        {
            this.impl = impl;
        }

        /// <summary>
        /// Gets the temporary path for schema generation, using the custom implementation if set, otherwise delegating to the single-parameter overload.
        /// </summary>
        /// <param name="schemaDefinition">The DAO schema definition.</param>
        /// <param name="typeSchema">The type schema, which may be ignored by the default implementation.</param>
        /// <returns>A file system path for temporary schema output.</returns>
        public virtual string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition, ITypeSchema? typeSchema)
        {
            if(impl != null)
            {
                return impl(schemaDefinition, typeSchema);
            }
            return GetSchemaTempPath(schemaDefinition); // This implementation ignores typeSchema
        }

        /// <summary>
        /// Gets the temporary path for schema generation based on the schema definition name, defaulting to a "DaoTemp_" prefixed subdirectory in the process data folder.
        /// </summary>
        /// <param name="schemaDefinition">The DAO schema definition.</param>
        /// <returns>A file system path for temporary schema output.</returns>
        public virtual string GetSchemaTempPath(IDaoSchemaDefinition schemaDefinition)
        {
            if (impl != null)
            {
                return impl(schemaDefinition, null);
            }
            return Path.Combine(RuntimeSettings.ProcessDataFolder, $"DaoTemp_{schemaDefinition.Name}");
        }
    }
}
