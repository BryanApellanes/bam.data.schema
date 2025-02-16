/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
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
