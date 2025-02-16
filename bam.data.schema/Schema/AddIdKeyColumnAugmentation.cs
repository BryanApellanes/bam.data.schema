/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public class AddIdKeyColumnAugmentation: AddColumnAugmentation
    {
        public AddIdKeyColumnAugmentation(bool caps = false)
            : base()
        {
            this.ColumnName = caps ? "ID": "Id";
            this.DataType = DataTypes.ULong;
            this.AllowNull = false;
        }
        public override void Execute(string tableName, DaoSchemaManager manager)
        {
            base.Execute(tableName, manager);
            manager.SetKeyColumn(tableName, ColumnName);
        }
    }
}
