/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A code generator that writes Dao code for a SchemaDefinition.
    /// </summary>
    public class DaoGenerator : IDaoGenerator
    {
        private readonly List<Stream> _resultStreams = new List<Stream>();

        /// <summary>
        /// Initializes a new instance of <see cref="DaoGenerator"/> with the specified code writer and a default namespace of "DaoGenerated".
        /// </summary>
        /// <param name="codeWriter">The code writer used to emit DAO source files.</param>
        public DaoGenerator(IDaoCodeWriter codeWriter)
        {
            this.DisposeOnComplete = true;
            this.SubscribeToEvents();

            this.Namespace = "DaoGenerated";
            this.DaoCodeWriter = codeWriter; 
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DaoGenerator"/> with the specified code writer and namespace.
        /// </summary>
        /// <param name="codeWriter">The code writer used to emit DAO source files.</param>
        /// <param name="nameSpace">The namespace for generated DAO classes.</param>
        public DaoGenerator(IDaoCodeWriter codeWriter, string nameSpace)
        {
            this.DisposeOnComplete = true;
            this.SubscribeToEvents();

            this.Namespace = nameSpace;
            this.DaoCodeWriter = codeWriter;
        }

        /// <summary>
        /// Gets or sets the code writer used to emit DAO source files.
        /// </summary>
        public IDaoCodeWriter DaoCodeWriter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether dispose will
        /// be called on the output streams after code generation.
        /// </summary>
        public bool DisposeOnComplete { get; set; }

        /// <summary>
        /// Gets or sets whether to generate Qi (query interface) classes alongside DAO classes.
        /// </summary>
        public bool GenerateQiClasses { get; set; }

        #region events

        /// <summary>
        /// The event that fires prior to code generation
        /// </summary>
        public event GeneratorEventDelegate GenerateStarted = null!;

        /// <summary>
        /// The event that fires when code generation is complete
        /// </summary>
        public event GeneratorEventDelegate GenerateComplete = null!;

        protected void OnGenerateStarted(IDaoSchemaDefinition schema)
        {
            GenerateStarted?.Invoke(this, schema);
        }

        protected void OnGenerateComplete(IDaoSchemaDefinition schema)
        {
            GenerateComplete?.Invoke(this, schema);
        }

        #endregion events

        /// <summary>
        /// If the generator compiled generated files, this will be the FileInfo 
        /// representing the compiled assembly
        /// </summary>
        public FileInfo DaoAssemblyFile { get; set; } = null!;

        /// <summary>
        /// Gets or sets the namespace to use for generated DAO classes.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Generates code for the specified schema using the current directory as the output root.
        /// </summary>
        /// <param name="schema">The schema to generate code for.</param>
        public void Generate(IDaoSchemaDefinition schema)
        {
            Generate(schema, "./");
        }

        /// <summary>
        /// Generate code for the specified schema
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="root"></param>
        public void Generate(IDaoSchemaDefinition schema, string root)
        {
            Generate(schema, null, root, null);
        }

        /// <summary>
        /// Generates code for the specified schema, writing output to the specified root directory and partial classes to the partials directory.
        /// </summary>
        /// <param name="schema">The schema to generate code for.</param>
        /// <param name="root">The root file path for generated output.</param>
        /// <param name="partialsDir">The directory for partial class files.</param>
        public void Generate(IDaoSchemaDefinition schema, string root, string partialsDir)
        {
            Generate(schema, null, root, partialsDir);
        }

        /// <summary>
        /// Generate code for the specified schema
        /// </summary>
        /// <param name="schema">The schema to generate code for</param>
        /// <param name="targetResolver">If specified, generated code will be 
        /// written to the stream returned by this function</param>
        /// <param name="root">The root file path to use if no target resolver is specified</param>
        public void Generate(IDaoSchemaDefinition schema, Func<string, Stream>? targetResolver = null, string root = "./", string? partialsDir = null)
        {
            if (string.IsNullOrEmpty(Namespace))
            {
                throw new NamespaceNotSpecifiedException();
            }
            DaoCodeWriter.Namespace = Namespace;

            OnGenerateStarted(schema);

            DaoCodeWriter.WriteContextClass(schema, targetResolver!, root);

            bool writePartial = !string.IsNullOrEmpty(partialsDir);
            if (writePartial)
            {
                EnsurePartialsDir(partialsDir!);
            }

            foreach (ITable table in schema.Tables)
            {
                if (writePartial)
                {
                    DaoCodeWriter.WritePartial(schema, targetResolver!, root, table);
                }
                DaoCodeWriter.WriteDaoClass(schema, targetResolver!, root, table);
                DaoCodeWriter.WriteQueryClass(schema, targetResolver!, root, table);
                DaoCodeWriter.WritePagedQueryClass(schema, targetResolver!, root, table);
                if (GenerateQiClasses)
                {
                    DaoCodeWriter.WriteQiClass(schema, targetResolver!, root, table);
                }
                DaoCodeWriter.WriteCollectionClass(schema, targetResolver!, root, table);
                DaoCodeWriter.WriteColumnsClass(schema, targetResolver!, root, table);
            }

            OnGenerateComplete(schema);
        }

        private static void EnsurePartialsDir(string partialsDir)
        {
            DirectoryInfo partials = new DirectoryInfo(partialsDir);
            if (!partials.Exists)
            {
                partials.Create();
            }
        }

        protected virtual void WritePartialToStream(string code, Stream s)
        {
            WriteToStream(code, s);
        }

        private static void WriteToStream(string text, Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                sw.Write(text);
                sw.Flush();
            }
        }

        private void SubscribeToEvents()
        {
            this.GenerateComplete += (g, schema) =>
            {
                if (DisposeOnComplete)
                {
                    foreach (Stream s in this._resultStreams)
                    {
                        s.Dispose();
                    }
                }

                this._resultStreams.Clear();
            };
        }
    }
}
