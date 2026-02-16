/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A name formatter that returns table and column names unchanged (echo/pass-through).
    /// </summary>
    public class EchoNameFormatter: INameFormatter
    {
        /// <summary>
        /// Returns the table name unchanged as the class name.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <returns>The table name unchanged.</returns>
        public string FormatClassName(string tableName)
        {
            return tableName;
        }

        /// <summary>
        /// Returns the column name unchanged as the property name.
        /// </summary>
        /// <param name="tableName">The table name (unused).</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>The column name unchanged.</returns>
        public string FormatPropertyName(string tableName, string columnName)
        {
            return columnName;
        }
    }
}
