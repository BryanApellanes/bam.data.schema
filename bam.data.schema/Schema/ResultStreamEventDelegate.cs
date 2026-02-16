/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Delegate for events that provide generated code and the target stream to write to.
    /// </summary>
    /// <param name="code">The generated code content.</param>
    /// <param name="stream">The stream to write the code to.</param>
    public delegate void ResultStreamEventDelegate(string code, Stream stream); 
    
}
