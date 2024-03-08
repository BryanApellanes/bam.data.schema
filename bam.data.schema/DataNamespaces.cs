using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Provides convention based namespaces for data related object definitions.
    /// </summary>
    public class DataNamespaces
    {
        public const string DefaultBaseNamespace = "ApplicationDataTypes";

        public DataNamespaces(): this(DefaultBaseNamespace) 
        { 
        }

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

        public string DaoNamespace 
        {
            get
            {
                return $"{BaseNamespace}.Dao";
            }
        }

        public string WrapperNamespace 
        {
            get
            {
                return $"{BaseNamespace}.Wrappers";
            }
        }

        public static DataNamespaces For<T>()
        {
            return For(typeof(T));
        }

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
