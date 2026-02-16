/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A schema manager that will automatically add a
    /// Id column to every table when generating a 
    /// schema and related Data Access Objects from a *.db.js
    /// file
    /// </summary>
    public class AutoIdSchemaManager: DaoSchemaManager
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AutoIdSchemaManager"/>, adding an Id key column augmentation.
        /// </summary>
        /// <param name="autoSave">Whether to automatically save schema changes to disk.</param>
        /// <param name="caps">If true, uses "ID" (uppercase); otherwise "Id".</param>
        public AutoIdSchemaManager(bool autoSave = true, bool caps = false):base(autoSave)
        {
            PreColumnAugmentations.Add(new AddIdKeyColumnAugmentation(caps));            
        }
    }
}
