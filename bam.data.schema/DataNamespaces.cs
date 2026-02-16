namespace Bam.Data.Schema
{
    /// <summary>
    /// Provides convention based namespaces for data related object definitions.
    /// </summary>
    public class DataNamespaces
    {
        /// <summary>
        /// The default base namespace used when no specific namespace is provided.
        /// </summary>
        public const string DefaultBaseNamespace = "ApplicationDataTypes";

        /// <summary>
        /// Initializes a new instance of <see cref="DataNamespaces"/> using the default base namespace.
        /// </summary>
        public DataNamespaces(): this(DefaultBaseNamespace)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataNamespaces"/> with the specified base namespace.
        /// </summary>
        /// <param name="baseNamespace">The base namespace from which DAO and wrapper namespaces are derived.</param>
        public DataNamespaces(string baseNamespace)
        {
            _baseNamespace = baseNamespace;
        }

        string _baseNamespace;
        /// <summary>
        /// Gets or sets the namespace that contains type definitions for storable data.
        /// </summary>
        public string BaseNamespace 
        {
            get 
            {
                return _baseNamespace;
            }
            set
            {
                _baseNamespace = value;

            }
        }

        /// <summary>
        /// Gets the namespace for generated DAO classes, derived by appending ".Dao" to <see cref="BaseNamespace"/>.
        /// </summary>
        public string DaoNamespace
        {
            get
            {
                return $"{BaseNamespace}.Dao";
            }
        }

        /// <summary>
        /// Gets the namespace for generated wrapper classes, derived by appending ".Wrappers" to <see cref="BaseNamespace"/>.
        /// </summary>
        public string WrapperNamespace
        {
            get
            {
                return $"{BaseNamespace}.Wrappers";
            }
        }

        /// <summary>
        /// Creates a <see cref="DataNamespaces"/> instance using the namespace of the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose namespace to use as the base namespace.</typeparam>
        /// <returns>A new <see cref="DataNamespaces"/> instance based on the type's namespace.</returns>
        public static DataNamespaces For<T>()
        {
            return For(typeof(T));
        }

        /// <summary>
        /// Creates a <see cref="DataNamespaces"/> instance using the namespace of the specified type.
        /// </summary>
        /// <param name="type">The type whose namespace to use as the base namespace.</param>
        /// <returns>A new <see cref="DataNamespaces"/> instance based on the type's namespace, or a default instance if the type or its namespace is null.</returns>
        public static DataNamespaces For(Type type)
        {
            if(type == null)
            {
                return new DataNamespaces();
            }

            if(type.Namespace == null)
            {
                return new DataNamespaces();
            }

            return new DataNamespaces(type.Namespace);
        }
    }
}
