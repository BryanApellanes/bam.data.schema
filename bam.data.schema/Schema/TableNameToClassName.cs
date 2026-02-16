/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Maps a database table name to its corresponding C# class name.
    /// </summary>
    public class TableNameToClassName
    {
        /// <summary>
        /// Implicitly converts to string by returning the <see cref="ClassName"/>.
        /// </summary>
        /// <param name="tntcn">The mapping to convert.</param>
        public static implicit operator string(TableNameToClassName tntcn)
        {
            return tntcn.ClassName;
        }

        /// <summary>
        /// Gets or sets the database table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the C# class name.
        /// </summary>
        public string ClassName { get; set; }
    }
}
