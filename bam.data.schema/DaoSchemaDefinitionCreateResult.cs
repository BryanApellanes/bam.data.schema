/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bam.Data.Schema;
using Bam.Net.Data.Schema;

namespace Bam.Net.Data.Repositories
{
    public sealed class DaoSchemaDefinitionCreateResult
	{
		public DaoSchemaDefinitionCreateResult(IDaoSchemaDefinition schemaDefinition, TypeSchema typeSchema, KeyColumn[] missingKeyColumns = null, ForeignKeyColumn[] missingForeignKeyColumns = null) 
		{
			this.DaoSchemaDefinition = schemaDefinition;
			this.TypeSchema = typeSchema;
			this.Warnings = new SchemaWarnings(missingKeyColumns, missingForeignKeyColumns);
		}

		public TypeSchema TypeSchema { get; private set; }
		public HashSet<ITypeSchemaWarning> TypeSchemaWarnings { get; set; }
		public IDaoSchemaDefinition DaoSchemaDefinition { get; private set; }
		public SchemaWarnings Warnings { get; private set; }

		public bool MissingColumns => Warnings.MissingKeyColumns.Length > 0 || Warnings.MissingForeignKeyColumns.Length > 0;
	}
}
