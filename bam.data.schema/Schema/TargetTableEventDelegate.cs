/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Delegate for events related to a target table during code generation.
    /// </summary>
    /// <param name="ns">The namespace for the generated code.</param>
    /// <param name="table">The table being processed.</param>
    public delegate void TargetTableEventDelegate(string ns, Table table);
    
}
