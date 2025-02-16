/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public interface INameFormatter
    {
        string FormatClassName(string tableName);
        string FormatPropertyName(string tableName, string columnName);
    }
}
