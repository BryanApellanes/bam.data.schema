/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Delegate for events raised during DAO code generation.
    /// </summary>
    /// <param name="generator">The generator raising the event.</param>
    /// <param name="schema">The schema definition being generated.</param>
    public delegate void GeneratorEventDelegate(DaoGenerator generator, IDaoSchemaDefinition schema);
    
}
