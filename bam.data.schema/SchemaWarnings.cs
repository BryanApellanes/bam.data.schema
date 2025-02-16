/*
	Copyright © Bryan Apellanes 2015  
*/

using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
	public class SchemaWarnings
	{
		public SchemaWarnings(KeyColumn[] missingKeyColumns, ForeignKeyColumn[] missingForeignKeyColumns) 
		{
			this.MissingKeyColumns = missingKeyColumns ?? new KeyColumn[] { };
			this.MissingForeignKeyColumns = missingForeignKeyColumns ?? new ForeignKeyColumn[] {};
		}

		public KeyColumn[] MissingKeyColumns { get; set; }
		public ForeignKeyColumn[] MissingForeignKeyColumns { get; set; }

	}
}
