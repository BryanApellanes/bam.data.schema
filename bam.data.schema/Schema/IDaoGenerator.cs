/*
	Copyright © Bryan Apellanes 2015  
*/
namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines the contract for a DAO code generator that writes source files from a schema definition.
    /// </summary>
    public interface IDaoGenerator
    {
        /// <summary>
        /// Gets or sets the file info of the compiled DAO assembly, if applicable.
        /// </summary>
        FileInfo DaoAssemblyFile { get; set; }

        /// <summary>
        /// Gets or sets the code writer used to emit DAO source files.
        /// </summary>
        IDaoCodeWriter DaoCodeWriter { get; set; }

        /// <summary>
        /// Gets or sets whether to dispose output streams after code generation completes.
        /// </summary>
        bool DisposeOnComplete { get; set; }

        /// <summary>
        /// Gets or sets whether to generate Qi (query interface) classes.
        /// </summary>
        bool GenerateQiClasses { get; set; }

        /// <summary>
        /// Gets or sets the namespace for generated DAO classes.
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// Occurs when code generation is complete.
        /// </summary>
        event GeneratorEventDelegate GenerateComplete;

        /// <summary>
        /// Occurs when code generation begins.
        /// </summary>
        event GeneratorEventDelegate GenerateStarted;

        /// <summary>
        /// Generates code for the specified schema using the current directory as the output root.
        /// </summary>
        /// <param name="schema">The schema to generate code for.</param>
        void Generate(IDaoSchemaDefinition schema);

        /// <summary>
        /// Generates code for the specified schema with full control over output resolution.
        /// </summary>
        /// <param name="schema">The schema to generate code for.</param>
        /// <param name="targetResolver">An optional function to resolve output streams by name.</param>
        /// <param name="root">The root file path for generated output.</param>
        /// <param name="partialsDir">The directory for partial class files.</param>
        void Generate(IDaoSchemaDefinition schema, Func<string, Stream>? targetResolver = null, string root = "./", string? partialsDir = null);

        /// <summary>
        /// Generates code for the specified schema, writing to the specified root directory.
        /// </summary>
        /// <param name="schema">The schema to generate code for.</param>
        /// <param name="root">The root file path for generated output.</param>
        void Generate(IDaoSchemaDefinition schema, string root);

        /// <summary>
        /// Generates code for the specified schema, writing to the specified root directory with partial class output.
        /// </summary>
        /// <param name="schema">The schema to generate code for.</param>
        /// <param name="root">The root file path for generated output.</param>
        /// <param name="partialsDir">The directory for partial class files.</param>
        void Generate(IDaoSchemaDefinition schema, string root, string partialsDir);
    }
}