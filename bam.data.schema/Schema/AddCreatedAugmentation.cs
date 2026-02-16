/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A schema manager augmentation that adds a non-nullable "Created" DateTime column to a table.
    /// </summary>
    public class AddCreatedAugmentation: AddColumnAugmentation
    {
        public AddCreatedAugmentation()
        {
            this.ColumnName = "Created";
            this.DataType = DataTypes.DateTime;
            this.AllowNull = false;
        }
    }
}
