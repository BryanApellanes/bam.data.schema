/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
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
