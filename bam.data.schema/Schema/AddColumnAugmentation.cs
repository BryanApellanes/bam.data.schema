/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A schema manager augmentation that adds a single column with a specified name, data type, and nullability to a table.
    /// </summary>
    public class AddColumnAugmentation: DaoSchemaManagerAugmentation
    {
        /// <summary>
        /// Gets or sets the name of the column to add.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the data type of the column to add.
        /// </summary>
        public DataTypes DataType { get; set; }

        /// <summary>
        /// Gets or sets whether the column allows null values.
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// Adds the configured column to the specified table using the schema manager.
        /// </summary>
        /// <param name="tableName">The name of the table to add the column to.</param>
        /// <param name="manager">The schema manager to use for adding the column.</param>
        public override void Execute(string tableName, DaoSchemaManager manager)
        {
            manager.AddColumn(tableName, new Column(ColumnName, DataType, AllowNull));
        }
    }
}
