using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    public class TypeSchemaException: Exception
    {
        public TypeSchemaException(params ITypeSchemaWarning[] warnings) : base(string.Join("\r\n", warnings.Select(w => w.ToString())))
        {
        }
    }
}