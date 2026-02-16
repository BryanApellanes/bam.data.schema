namespace Bam.Data.Schema
{
    /// <summary>
    /// A model that wraps a non-foreign-key column for use in code generation templates.
    /// </summary>
    public class NonForeignKeyColumnModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NonForeignKeyColumnModel"/> wrapping the specified column.
        /// </summary>
        /// <param name="column">The column to wrap.</param>
        public NonForeignKeyColumnModel(IColumn column)
        {
            Column = column;
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped column is a key column.
        /// </summary>
        public bool Key => Column is KeyColumn;

        /// <summary>
        /// Gets or sets the underlying column.
        /// </summary>
        public IColumn Column { get; set; }

        /// <summary>
        /// Gets the class name of the table this column belongs to.
        /// </summary>
        public string TableClassName => Column.TableClassName;

        /// <summary>
        /// Gets the C# property name for this column.
        /// </summary>
        public string PropertyName => Column.PropertyName;

        /// <summary>
        /// Gets the database column name.
        /// </summary>
        public string Name => Column.Name;

        /// <summary>
        /// Gets the database data type string.
        /// </summary>
        public string DbDataType => Column.DbDataType;

        /// <summary>
        /// Gets the maximum length for the column.
        /// </summary>
        public string MaxLength => Column.MaxLength;

        /// <summary>
        /// Gets a lowercase string indicating whether the column allows null.
        /// </summary>
        public string AllowNull => Column.AllowNull.ToString().ToLowerInvariant();

        /// <summary>
        /// Gets the native C# type string for this column.
        /// </summary>
        public string NativeType => Column.NativeType;

        /// <summary>
        /// Gets the DAO data type as a string.
        /// </summary>
        public string DataType => Column.DataType.ToString();
    }
}
