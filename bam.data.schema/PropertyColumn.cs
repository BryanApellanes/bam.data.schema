using System.Reflection;
using Newtonsoft.Json;
using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Maps a CLR property to a database column, determining the column name and data type from the property's attributes and type.
    /// </summary>
    public class PropertyColumn
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PropertyColumn"/> from the specified property, deriving the column name and data type.
        /// </summary>
        /// <param name="property">The CLR property to map to a database column.</param>
        public PropertyColumn(PropertyInfo property)
        {
            PropertyInfo = property;
            string columnName = property.Name.LettersOnly();
            ColumnAttribute? attr = null;
            if(property.HasCustomAttributeOfType<ColumnAttribute>(out attr))
            {
                columnName = attr.Name;
            }
            Column = new Column(columnName, SchemaProvider.GetColumnDataType(property));
            if(attr != null)
            {
                Column.AllowNull = attr.AllowNull;
            }
        }

        /// <summary>
        /// Gets the underlying CLR property info for this column mapping.
        /// </summary>
        [JsonIgnore]
        [Exclude]
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets or sets the database column definition derived from the property.
        /// </summary>
        public Column Column { get; set; }

        /// <summary>
        /// Returns a string representation of this property-column mapping, showing the column name.
        /// </summary>
        /// <returns>A string containing the column name in curly braces.</returns>
        public override string ToString()
        {
            return $"{{{Column.Name}}}";
        }
    }
}
