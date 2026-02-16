/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A Schema Manager augmentation that will 
    /// add a Created and Modified column and 
    /// optionally a ModifiedBy column.
    /// </summary>
    public class AddAuditColumnsAugmentation : DaoSchemaManagerAugmentation
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AddAuditColumnsAugmentation"/>.
        /// </summary>
        public AddAuditColumnsAugmentation()
        {

        }

        /// <summary>
        /// Gets or sets whether to add a ModifiedBy column.
        /// </summary>
        public bool IncludeModifiedBy { get; set; }

		/// <summary>
		/// Gets or sets whether to add a CreatedBy column.
		/// </summary>
		public bool IncludeCreatedBy { get; set; }

        /// <summary>
        /// Adds Created, Modified, and optionally ModifiedBy and CreatedBy columns to the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to augment.</param>
        /// <param name="manager">The schema manager to add columns to.</param>
        public override void Execute(string tableName, DaoSchemaManager manager)
        {
            manager.AddColumn(tableName, new Column("Created", DataTypes.DateTime, false));
            manager.AddColumn(tableName, new Column("Modified", DataTypes.DateTime, false));
            if (IncludeModifiedBy)
            {
                manager.AddColumn(tableName, new Column("ModifiedBy", DataTypes.String, false));
            }
			if (IncludeCreatedBy)
			{
				manager.AddColumn(tableName, new Column("CreatedBy", DataTypes.String, false));
			}
        }
    }
}
