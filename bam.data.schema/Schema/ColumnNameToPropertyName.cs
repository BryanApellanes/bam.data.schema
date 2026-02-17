/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Maps a database column name to its corresponding C# property name for a given table.
    /// </summary>
    public class ColumnNameToPropertyName
    {
        /// <summary>
        /// Implicitly converts to string by returning the <see cref="PropertyName"/>.
        /// </summary>
        /// <param name="cntpn">The mapping to convert.</param>
        public static implicit operator string(ColumnNameToPropertyName cntpn)
        {
            return cntpn.PropertyName;
        }

        /// <summary>
        /// Gets or sets the database table name.
        /// </summary>
        public string TableName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the database column name.
        /// </summary>
        public string ColumnName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the C# property name.
        /// </summary>
        public string PropertyName { get; set; } = null!;
    }
}
