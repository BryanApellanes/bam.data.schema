/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A schema manager augmentation that adds a non-nullable "Modified" DateTime column to a table.
    /// </summary>
    public class AddModifiedAugmentation: AddColumnAugmentation
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AddModifiedAugmentation"/> with "Modified" as the column name.
        /// </summary>
        public AddModifiedAugmentation()
        {
            this.ColumnName = "Modified";
            this.DataType = DataTypes.DateTime;
            this.AllowNull = false;
        }
    }
}
