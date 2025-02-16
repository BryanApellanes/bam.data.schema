/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public class TableNameToClassName
    {
        public static implicit operator string(TableNameToClassName tntcn)
        {
            return tntcn.ClassName;
        }
        public string TableName { get; set; }
        public string ClassName { get; set; }
    }
}
