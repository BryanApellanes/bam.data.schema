using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    public static class TypeExtensions
    {

        public static bool ExtendsType(this Type type, Type extends)
        {
            if (type == extends)
            {
                return false;
            }

            TypeInheritanceDescriptor descriptor = new TypeInheritanceDescriptor(type);
            return descriptor.Extends(extends);
        }
    }
}
