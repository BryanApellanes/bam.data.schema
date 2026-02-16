/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A class used to de-sillyfy database naming conventions
    /// </summary>
    public class SchemaNameMap
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="SchemaNameMap"/>.
        /// </summary>
        public SchemaNameMap()
        {
            this.TableNamesToClassNames = new List<TableNameToClassName>();
            this.ColumnNamesToPropertyNames = new List<ColumnNameToPropertyName>();
        }

        /// <summary>
        /// Gets or sets the list of table name to class name mappings.
        /// </summary>
        public List<TableNameToClassName> TableNamesToClassNames { get; set; }

        /// <summary>
        /// Gets or sets the list of column name to property name mappings.
        /// </summary>
        public List<ColumnNameToPropertyName> ColumnNamesToPropertyNames { get; set; }

        /// <summary>
        /// Loads a <see cref="SchemaNameMap"/> from the specified JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>The loaded schema name map.</returns>
        public static SchemaNameMap Load(string filePath)
        {
            return filePath.FromJsonFile<SchemaNameMap>();
        }
        
        /// <summary>
        /// Saves this schema name map to the specified JSON file.
        /// </summary>
        /// <param name="filePath">The path to save to.</param>
        public void Save(string filePath)
        {
            this.ToJsonFile(filePath);
        }

        /// <summary>
        /// Gets the database table name for the specified class name, or returns the class name if no mapping exists.
        /// </summary>
        /// <param name="className">The C# class name to look up.</param>
        /// <returns>The corresponding table name.</returns>
        public string GetTableName(string className)
        {
            TableNameToClassName? lookup = TableNamesToClassNames.FirstOrDefault(t => t.ClassName.Equals(className));
            if (lookup != null)
            {
                return lookup.TableName;
            }
            return className;
        }

        /// <summary>
        /// Gets the C# class name for the specified table name, or returns the table name if no mapping exists.
        /// </summary>
        /// <param name="tableName">The database table name to look up.</param>
        /// <returns>The corresponding class name.</returns>
        public string GetClassName(string tableName)
        {
            TableNameToClassName? lookup = TableNamesToClassNames.FirstOrDefault(t => t.TableName.Equals(tableName));
            if (lookup != null)
            {
                return lookup.TableName;
            }
            return tableName;
        }
        
        /// <summary>
        /// Gets the database column name for the specified class and property names.
        /// </summary>
        /// <param name="className">The C# class name.</param>
        /// <param name="propertyName">The C# property name.</param>
        /// <returns>The corresponding column name, or the property name if no mapping exists.</returns>
        public string GetColumnName(string className, string propertyName)
        {
            string tableName = GetTableName(className);
            ColumnNameToPropertyName? lookup = ColumnNamesToPropertyNames.FirstOrDefault(c => c.TableName.Equals(tableName) && c.PropertyName.Equals(propertyName));
            if (lookup != null)
            {
                return lookup.ColumnName;
            }

            return propertyName;
        }

        /// <summary>
        /// Gets the C# property name for the specified table and column names.
        /// </summary>
        /// <param name="tableName">The database table name.</param>
        /// <param name="columnName">The database column name.</param>
        /// <returns>The corresponding property name, or the column name if no mapping exists.</returns>
        public string GetPropertyName(string tableName, string columnName)
        {
            ColumnNameToPropertyName? lookup = ColumnNamesToPropertyNames.FirstOrDefault(c => c.TableName.Equals(tableName) && c.ColumnName.Equals(columnName));
            if (lookup != null)
            {
                return lookup.PropertyName;
            }

            return columnName;
        }

        /// <summary>
        /// Adds or replaces a table name to class name mapping.
        /// </summary>
        /// <param name="tableNameToClassName">The mapping to set.</param>
        public void Set(TableNameToClassName tableNameToClassName)
        {
            Remove(tableNameToClassName, true);
            Remove(tableNameToClassName, false);
            TableNamesToClassNames.Add(tableNameToClassName);
        }
        /// <summary>
        /// Adds or replaces a column name to property name mapping.
        /// </summary>
        /// <param name="columnNameToPropertyName">The mapping to set.</param>
        public void Set(ColumnNameToPropertyName columnNameToPropertyName)
        {
            Remove(columnNameToPropertyName, true);
            Remove(columnNameToPropertyName, false);
            ColumnNamesToPropertyNames.Add(columnNameToPropertyName);
        }

        private void Remove(ColumnNameToPropertyName columnNameToPropertyName, bool favorTable)
        {
            // favoring single return queries for readability; despite performance hit
            ColumnNameToPropertyName? toRemove = favorTable ? ColumnNamesToPropertyNames.FirstOrDefault(c => c.TableName.Equals(columnNameToPropertyName.TableName) && c.ColumnName.Equals(columnNameToPropertyName.ColumnName)) : ColumnNamesToPropertyNames.FirstOrDefault(c => c.TableName.Equals(columnNameToPropertyName.TableName) && c.PropertyName.Equals(columnNameToPropertyName.PropertyName));
            if (toRemove != null)
            {
                ColumnNamesToPropertyNames.Remove(toRemove);
            }
        }

        private void Remove(TableNameToClassName tableNameToClassName, bool favorTable)
        {
            // favoring single return queries for readability; despite performance hit
            TableNameToClassName? toRemove = favorTable ? TableNamesToClassNames.FirstOrDefault(c => c.TableName.Equals(tableNameToClassName.TableName)) : TableNamesToClassNames.FirstOrDefault(c => c.ClassName.Equals(tableNameToClassName.ClassName));
            if (toRemove != null)
            {
                TableNamesToClassNames.Remove(toRemove);
            }
        }
    }
}
