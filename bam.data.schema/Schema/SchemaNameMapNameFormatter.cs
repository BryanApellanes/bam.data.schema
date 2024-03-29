/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Data.Schema
{
    /// <summary>
    /// A name formatter that uses a SchemaNameMap to 
    /// name classes and properties
    /// </summary>
    public class SchemaNameMapNameFormatter: INameFormatter
    {
        public SchemaNameMapNameFormatter() { }
        public SchemaNameMapNameFormatter(SchemaNameMap nameMap)
        {
            this.NameMap = nameMap;
        }

        public SchemaNameMap NameMap { get; set; }
        public string FormatClassName(string tableName)
        {
            if (NameMap != null)
            {
                return NameMap.GetClassName(tableName);
            }
            return tableName;
        }

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
