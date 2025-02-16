/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Augments the behavior of a SchemaManager.
    /// </summary>
    public abstract class DaoSchemaManagerAugmentation
    {
        public abstract void Execute(string tableName, DaoSchemaManager manager);
    }
}
