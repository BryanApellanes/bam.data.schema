using Newtonsoft.Json;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Used to describe the shape of a database table
    /// given a CLR type.
    /// </summary>
    public class TypeTable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TypeTable"/> for the specified CLR type, extracting its declared properties as columns.
        /// </summary>
        /// <param name="type">The CLR type to describe as a database table.</param>
        public TypeTable(Type type)
        {
            Type = type;
            PropertyColumns = GetColumns();
        }

        /// <summary>
        /// Gets or sets the CLR type this table represents.
        /// </summary>
        [JsonIgnore]
        [Exclude]
        public Type Type { get; set; }

        /// <summary>
        /// Gets the table name for this type using the specified table name provider.
        /// </summary>
        /// <param name="tableNameProvider">The provider to use for table name resolution; defaults to <see cref="EchoTypeTableNameProvider"/> if null.</param>
        /// <returns>The table name for this type.</returns>
        public string GetTableName(ITypeTableNameProvider tableNameProvider = null)
        {
            tableNameProvider = tableNameProvider ?? new EchoTypeTableNameProvider();
            return tableNameProvider.GetTableName(Type);
        }
        /// <summary>
        /// Gets or sets the property-to-column mappings for properties declared directly on this type (not inherited).
        /// </summary>
        public PropertyColumn[] PropertyColumns { get; set; }
        private PropertyColumn[] GetColumns()
        {
            return Type.GetProperties().Where(pi => pi.DeclaringType == Type).Select(pi => new PropertyColumn(pi)).ToArray();
        }
    }
}
