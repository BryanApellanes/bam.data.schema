namespace Bam.Data.Schema
{
    /// <summary>
    /// A model that wraps a <see cref="ForeignKeyColumn"/> with a namespace for use in code generation templates.
    /// </summary>
    public class ForeignKeyColumnModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ForeignKeyColumnModel"/> with the specified foreign key column and namespace.
        /// </summary>
        /// <param name="fk">The foreign key column to wrap.</param>
        /// <param name="nameSpace">The namespace for the generated code.</param>
        public ForeignKeyColumnModel(ForeignKeyColumn fk, string nameSpace)
        {
            ForeignKeyColumn = fk;
            Namespace = nameSpace;
        }

        public ForeignKeyColumn ForeignKeyColumn { get; set; }

        public string Namespace { get; set; }

        public string Name { get { return ForeignKeyColumn.Name; } }
        public string TableName { get { return ForeignKeyColumn.TableName; } }
        public string DbDataType { get { return ForeignKeyColumn.DbDataType; } }
        public string MaxLength { get { return ForeignKeyColumn.MaxLength; } }
        public string AllowNull { get { return ForeignKeyColumn.AllowNull.ToString().ToLowerInvariant(); } }

        public string ReferencedKey { get { return ForeignKeyColumn.ReferencedKey; } }
        public string ReferencedTable { get { return ForeignKeyColumn.ReferencedTable; } }
        public string ReferenceNameSuffix { get { return ForeignKeyColumn.ReferenceNameSuffix; } }
        public string NativeType { get { return ForeignKeyColumn.NativeType; } }
        public string PropertyName { get { return ForeignKeyColumn.PropertyName; } }
        public string DataType { get { return ForeignKeyColumn.DataType.ToString(); } }
        public string ReferencedClass { get { return ForeignKeyColumn.ReferencedClass; } }
        public string CamelCaseReferencedClass { get { return ForeignKeyColumn.ReferencedClass.CamelCase(); } }
    }
}
