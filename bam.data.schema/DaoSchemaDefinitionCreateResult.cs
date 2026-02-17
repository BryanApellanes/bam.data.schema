/*
	Copyright © Bryan Apellanes 2015  
*/

using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Represents the result of creating a DAO schema definition from CLR types, including the generated schema, type schema, and any warnings about missing columns.
    /// </summary>
    public sealed class DaoSchemaDefinitionCreateResult
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DaoSchemaDefinitionCreateResult"/> with the specified schema definition, type schema, and optional missing column information.
		/// </summary>
		/// <param name="schemaDefinition">The generated DAO schema definition.</param>
		/// <param name="typeSchema">The type schema describing CLR type relationships.</param>
		/// <param name="missingKeyColumns">Key columns that were inferred because they were not found on the CLR types.</param>
		/// <param name="missingForeignKeyColumns">Foreign key columns that were inferred because they were not found on the CLR types.</param>
		public DaoSchemaDefinitionCreateResult(IDaoSchemaDefinition schemaDefinition, TypeSchema typeSchema, KeyColumn[]? missingKeyColumns = null, ForeignKeyColumn[]? missingForeignKeyColumns = null)
		{
			this.DaoSchemaDefinition = schemaDefinition;
			this.TypeSchema = typeSchema;
			this.Warnings = new SchemaWarnings(missingKeyColumns!, missingForeignKeyColumns!);
		}

		/// <summary>
		/// Gets the type schema that describes database-like relationships between CLR types.
		/// </summary>
		public TypeSchema TypeSchema { get; private set; }

		/// <summary>
		/// Gets or sets the set of warnings generated during type schema creation.
		/// </summary>
		public HashSet<ITypeSchemaWarning> TypeSchemaWarnings { get; set; } = null!;

		/// <summary>
		/// Gets the generated DAO schema definition containing tables, columns, and foreign keys.
		/// </summary>
		public IDaoSchemaDefinition DaoSchemaDefinition { get; private set; }

		/// <summary>
		/// Gets the schema warnings containing any missing key or foreign key columns that were inferred.
		/// </summary>
		public SchemaWarnings Warnings { get; private set; }

		/// <summary>
		/// Gets a value indicating whether any key columns or foreign key columns were missing from the CLR types and had to be inferred.
		/// </summary>
		public bool MissingColumns => Warnings.MissingKeyColumns.Length > 0 || Warnings.MissingForeignKeyColumns.Length > 0;
	}
}
