/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A key/identity column.
    /// </summary>
    public partial class KeyColumn: Column
    {
		/// <summary>
		/// Initializes a new empty instance of <see cref="KeyColumn"/>.
		/// </summary>
		public KeyColumn() { }

        /// <summary>
        /// Initializes a new instance of <see cref="KeyColumn"/> by copying the specified column's metadata.
        /// </summary>
        /// <param name="c">The column to copy name, data type, db data type, and max length from.</param>
        public KeyColumn(IColumn c)
            : base(c.TableName)
        {
            this.Name = c.Name;
            this.DataType = c.DataType;
            this.DbDataType = c.DbDataType;
            this.MaxLength = c.MaxLength;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="KeyColumn"/> with the specified name and data type. AllowNull is always false for key columns.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="type">The data type of the column.</param>
        /// <param name="allowNull">Ignored; key columns never allow null.</param>
        public KeyColumn(string name, DataTypes type, bool allowNull = true)
        {
            this.Name = name;
            this.DataType = type;
            this.AllowNull = allowNull;
        }

        /// <summary>
        /// Gets a value indicating whether this column allows null. Always returns false for key columns.
        /// </summary>
        public override bool AllowNull
        {
            get
            {
                return false;
            }
            set
            {
                // no nulls if key column
            }
        }

        /// <summary>
        /// Gets a value indicating whether this column is a key. Always returns true for key columns.
        /// </summary>
        public override bool Key
        {
            get
            {
                return true;
            }
            set
            {
                // setter for deserialization only, key is always true
            }
        }
        /// <summary>
        /// Gets the default key column with name "Id" and data type ULong.
        /// </summary>
        public static KeyColumn Default { get; } = new KeyColumn("Id", DataTypes.ULong);

    }
}
