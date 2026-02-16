using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Provides extension methods for CLR types related to type inheritance analysis.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type extends (inherits from) the given base type, excluding self-comparison.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="extends">The potential base type.</param>
        /// <returns>True if <paramref name="type"/> inherits from <paramref name="extends"/> and is not the same type; otherwise false.</returns>
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
