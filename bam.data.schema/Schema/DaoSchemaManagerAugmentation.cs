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
        /// <summary>
        /// Executes the augmentation against the specified table using the given schema manager.
        /// </summary>
        /// <param name="tableName">The name of the table to augment.</param>
        /// <param name="manager">The schema manager to use for applying augmentations.</param>
        public abstract void Execute(string tableName, DaoSchemaManager manager);
    }
}
