/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public class AddColumnAugmentation: DaoSchemaManagerAugmentation
    {
        public string ColumnName { get; set; }
        public DataTypes DataType { get; set; }
        public bool AllowNull { get; set; }
        public override void Execute(string tableName, DaoSchemaManager manager)
        {
            manager.AddColumn(tableName, new Column(ColumnName, DataType, AllowNull));
        }
    }
}
