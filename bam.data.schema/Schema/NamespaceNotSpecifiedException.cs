/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Exception thrown when a DAO generation operation is attempted without specifying a namespace.
    /// </summary>
    public class NamespaceNotSpecifiedException: Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NamespaceNotSpecifiedException"/> with a default message.
        /// </summary>
        public NamespaceNotSpecifiedException() : base("Namespace was not specified") { }
    }
}
