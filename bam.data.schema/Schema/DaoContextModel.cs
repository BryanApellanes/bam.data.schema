namespace Bam.Data.Schema
{
    /// <summary>
    /// A model that wraps a DAO schema definition with a namespace, providing table schema models for code generation.
    /// </summary>
    public class DaoContextModel
    {
        /// <summary>
        /// Gets or sets the underlying DAO schema definition.
        /// </summary>
        public IDaoSchemaDefinition Model { get; set; } = null!;

        /// <summary>
        /// Gets or sets the namespace to use for generated code.
        /// </summary>
        public string Namespace { get; set; } = null!;

        /// <summary>
        /// Gets the table schema models for each table in the schema definition.
        /// </summary>
        public DaoTableSchemaModel[] Tables
        {
            get
            {
                return Model.Tables.Select(t => new DaoTableSchemaModel { Model = t, Namespace = Namespace, Schema = Model }).ToArray();
            }
        }
    }
}
