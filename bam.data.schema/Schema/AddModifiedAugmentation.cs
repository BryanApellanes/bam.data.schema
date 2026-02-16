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
        public AddModifiedAugmentation()
        {
            this.ColumnName = "Modified";
            this.DataType = DataTypes.DateTime;
            this.AllowNull = false;
        }
    }
}
