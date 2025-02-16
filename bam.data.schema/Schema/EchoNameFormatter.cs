/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public class EchoNameFormatter: INameFormatter
    {
        public string FormatClassName(string tableName)
        {
            return tableName;
        }

        public string FormatPropertyName(string tableName, string columnName)
        {
            return columnName;
        }
    }
}
