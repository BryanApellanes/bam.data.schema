using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Exception thrown when type schema generation encounters warnings that are treated as errors.
    /// </summary>
    public class TypeSchemaException: Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TypeSchemaException"/> with a message composed from the specified warnings.
        /// </summary>
        /// <param name="warnings">The type schema warnings that caused this exception.</param>
        public TypeSchemaException(params ITypeSchemaWarning[] warnings) : base(string.Join("\r\n", warnings.Select(w => w.ToString())))
        {
        }
    }
}