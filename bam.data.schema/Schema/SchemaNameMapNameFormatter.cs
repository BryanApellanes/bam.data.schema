/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A name formatter that uses a SchemaNameMap to 
    /// name classes and properties
    /// </summary>
    public class SchemaNameMapNameFormatter: INameFormatter
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="SchemaNameMapNameFormatter"/>.
        /// </summary>
        public SchemaNameMapNameFormatter() { }

        /// <summary>
        /// Initializes a new instance of <see cref="SchemaNameMapNameFormatter"/> with the specified name map.
        /// </summary>
        /// <param name="nameMap">The schema name map to use for lookups.</param>
        public SchemaNameMapNameFormatter(SchemaNameMap nameMap)
        {
            this.NameMap = nameMap;
        }

        /// <summary>
        /// Gets or sets the schema name map used for lookups.
        /// </summary>
        public SchemaNameMap NameMap { get; set; }

        /// <inheritdoc />
        public string FormatClassName(string tableName)
        {
            if (NameMap != null)
            {
                return NameMap.GetClassName(tableName);
            }
            return tableName;
        }

        /// <inheritdoc />
        public string FormatPropertyName(string tableName, string columnName)
        {
            if (NameMap != null)
            {
                return NameMap.GetPropertyName(tableName, columnName);
            }
            return columnName;
        }
    }
}
