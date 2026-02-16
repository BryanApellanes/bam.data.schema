namespace Bam.Data.Schema
{
    /// <summary>
    /// A model that wraps a referencing foreign key column, providing a property name for code generation.
    /// </summary>
    public class ReferencingForeignKeyModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReferencingForeignKeyModel"/> wrapping the specified foreign key.
        /// </summary>
        /// <param name="foreignKey">The referencing foreign key column.</param>
        public ReferencingForeignKeyModel(IForeignKeyColumn foreignKey)
        {
            Model = foreignKey;
        }

        /// <summary>
        /// Gets or sets the underlying foreign key column.
        /// </summary>
        public IForeignKeyColumn Model { get; set; }

        /// <summary>
        /// Gets the property name for the referencing collection, formatted as "{PluralizedReferencingClass}By{ForeignKeyName}".
        /// </summary>
        public string PropertyName
        {
            get
            {
                return $"{Model.ReferencingClass.Pluralize()}By{Model.Name}";
            }
        }
    }
}
