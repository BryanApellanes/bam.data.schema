/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Data.Schema
{
    public interface INameFormatter
    {
        string FormatClassName(string tableName);
        string FormatPropertyName(string tableName, string columnName);
    }
}
