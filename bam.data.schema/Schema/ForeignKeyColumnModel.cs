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

        /// <summary>
        /// Gets or sets the underlying foreign key column.
        /// </summary>
        public ForeignKeyColumn ForeignKeyColumn { get; set; }

        /// <summary>
        /// Gets or sets the namespace for the generated code.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Name { get { return ForeignKeyColumn.Name; } }

        /// <summary>
        /// Gets the table name this column belongs to.
        /// </summary>
        public string TableName { get { return ForeignKeyColumn.TableName; } }

        /// <summary>
        /// Gets the database data type string.
        /// </summary>
        public string DbDataType { get { return ForeignKeyColumn.DbDataType; } }

        /// <summary>
        /// Gets the maximum length for the column.
        /// </summary>
        public string MaxLength { get { return ForeignKeyColumn.MaxLength; } }

        /// <summary>
        /// Gets a lowercase string indicating whether the column allows null.
        /// </summary>
        public string AllowNull { get { return ForeignKeyColumn.AllowNull.ToString().ToLowerInvariant(); } }

        /// <summary>
        /// Gets the name of the key column in the referenced table.
        /// </summary>
        public string ReferencedKey { get { return ForeignKeyColumn.ReferencedKey; } }

        /// <summary>
        /// Gets the name of the referenced table.
        /// </summary>
        public string ReferencedTable { get { return ForeignKeyColumn.ReferencedTable; } }

        /// <summary>
        /// Gets the reference name suffix for disambiguation.
        /// </summary>
        public string ReferenceNameSuffix { get { return ForeignKeyColumn.ReferenceNameSuffix; } }

        /// <summary>
        /// Gets the native C# type string for the column.
        /// </summary>
        public string NativeType { get { return ForeignKeyColumn.NativeType; } }

        /// <summary>
        /// Gets the C# property name for the column.
        /// </summary>
        public string PropertyName { get { return ForeignKeyColumn.PropertyName; } }

        /// <summary>
        /// Gets the DAO data type as a string.
        /// </summary>
        public string DataType { get { return ForeignKeyColumn.DataType.ToString(); } }

        /// <summary>
        /// Gets the C# class name of the referenced table.
        /// </summary>
        public string ReferencedClass { get { return ForeignKeyColumn.ReferencedClass; } }

        /// <summary>
        /// Gets the camelCased version of the referenced class name.
        /// </summary>
        public string CamelCaseReferencedClass { get { return ForeignKeyColumn.ReferencedClass.CamelCase(); } }
    }
}
