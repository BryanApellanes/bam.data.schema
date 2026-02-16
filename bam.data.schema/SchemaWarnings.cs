/*
	Copyright © Bryan Apellanes 2015  
*/

using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
	/// <summary>
	/// Contains warnings about key and foreign key columns that were not found on CLR types and had to be inferred during schema generation.
	/// </summary>
	public class SchemaWarnings
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SchemaWarnings"/> with the specified missing columns.
		/// </summary>
		/// <param name="missingKeyColumns">Key columns that were inferred because they were not found on CLR types.</param>
		/// <param name="missingForeignKeyColumns">Foreign key columns that were inferred because they were not found on CLR types.</param>
		public SchemaWarnings(KeyColumn[] missingKeyColumns, ForeignKeyColumn[] missingForeignKeyColumns)
		{
			this.MissingKeyColumns = missingKeyColumns ?? new KeyColumn[] { };
			this.MissingForeignKeyColumns = missingForeignKeyColumns ?? new ForeignKeyColumn[] {};
		}

		/// <summary>
		/// Gets or sets the key columns that were inferred during schema generation.
		/// </summary>
		public KeyColumn[] MissingKeyColumns { get; set; }

		/// <summary>
		/// Gets or sets the foreign key columns that were inferred during schema generation.
		/// </summary>
		public ForeignKeyColumn[] MissingForeignKeyColumns { get; set; }

	}
}
