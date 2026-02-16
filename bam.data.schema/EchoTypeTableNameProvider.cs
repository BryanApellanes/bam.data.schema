namespace Bam.Data.Repositories
{
    /// <summary>
    /// A TypeTableNameProvider implementation that returns
    /// the name of the specified type with leading and trailing
    /// non letters removed
    /// </summary>
    public class EchoTypeTableNameProvider : ITypeTableNameProvider
    {
        /// <summary>
        /// Gets the table name for the specified type by returning the type name with leading and trailing non-letter characters removed.
        /// </summary>
        /// <param name="type">The type to get a table name for.</param>
        /// <returns>The type name with non-letter characters trimmed from both ends.</returns>
        public string GetTableName(Type type)
        {
            return type.Name.TrimNonLetters();
        }
    }
}
