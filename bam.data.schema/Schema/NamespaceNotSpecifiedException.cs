/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public class NamespaceNotSpecifiedException: Exception
    {
        public NamespaceNotSpecifiedException() : base("Namespace was not specified") { }
    }
}
