/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines the contract for formatting database table and column names into C# class and property names.
    /// </summary>
    public interface INameFormatter
    {
        /// <summary>
        /// Formats a database table name into a C# class name.
        /// </summary>
        /// <param name="tableName">The database table name.</param>
        /// <returns>The formatted class name.</returns>
        string FormatClassName(string tableName);

        /// <summary>
        /// Formats a database column name into a C# property name.
        /// </summary>
        /// <param name="tableName">The database table name containing the column.</param>
        /// <param name="columnName">The database column name.</param>
        /// <returns>The formatted property name.</returns>
        string FormatPropertyName(string tableName, string columnName);
    }
}
