/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
	public class DaoSchemaExtractorEventArgs: EventArgs
	{
		public DaoSchemaExtractorEventArgs() {}
		public string Table { get; set; }
		public string Column { get; set; }
        public string Property { get; set; }
        public ForeignKeyColumn ForeignKeyColumn { get; set; }
	}
}
