/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
	/// <summary>
	/// Event arguments for schema extraction events, containing information about the table, column, property, or foreign key being processed.
	/// </summary>
	public class DaoSchemaExtractorEventArgs: EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DaoSchemaExtractorEventArgs"/>.
		/// </summary>
		public DaoSchemaExtractorEventArgs() {}

		/// <summary>
		/// Gets or sets the name of the table being processed.
		/// </summary>
		public string Table { get; set; } = null!;

		/// <summary>
		/// Gets or sets the name of the column being processed.
		/// </summary>
		public string Column { get; set; } = null!;

		/// <summary>
		/// Gets or sets the property name involved in a naming collision.
		/// </summary>
        public string Property { get; set; } = null!;

		/// <summary>
		/// Gets or sets the foreign key column being processed.
		/// </summary>
        public ForeignKeyColumn ForeignKeyColumn { get; set; } = null!;
	}
}
