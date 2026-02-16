/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A schema manager augmentation that adds a non-nullable ULong "Id" key column and sets it as the primary key for the table.
    /// </summary>
    public class AddIdKeyColumnAugmentation: AddColumnAugmentation
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AddIdKeyColumnAugmentation"/>, optionally using uppercase "ID" for the column name.
        /// </summary>
        /// <param name="caps">If true, the column name is "ID"; otherwise "Id".</param>
        public AddIdKeyColumnAugmentation(bool caps = false)
            : base()
        {
            this.ColumnName = caps ? "ID": "Id";
            this.DataType = DataTypes.ULong;
            this.AllowNull = false;
        }
        /// <summary>
        /// Adds the Id column and sets it as the key column for the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to augment.</param>
        /// <param name="manager">The schema manager to use.</param>
        public override void Execute(string tableName, DaoSchemaManager manager)
        {
            base.Execute(tableName, manager);
            manager.SetKeyColumn(tableName, ColumnName);
        }
    }
}
