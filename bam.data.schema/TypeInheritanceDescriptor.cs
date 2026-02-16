using Bam.Data.Repositories;
using System.Text;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Describes the inheritance chain of a CLR type, breaking it into a list of <see cref="TypeTable"/> entries from the most derived type to the root base type.
    /// </summary>
    public class TypeInheritanceDescriptor
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="TypeInheritanceDescriptor"/>.
        /// </summary>
        public TypeInheritanceDescriptor() { }

        /// <summary>
        /// Initializes a new instance of <see cref="TypeInheritanceDescriptor"/> by walking the inheritance chain of the specified type up to (but not including) <see cref="object"/>.
        /// </summary>
        /// <param name="type">The type to describe the inheritance chain for.</param>
        public TypeInheritanceDescriptor(Type type)
        {
            Type = type;
            RootType = type;
            Chain = new List<TypeTable> { new TypeTable(type) };
            Type baseType = type.BaseType;
            while (baseType != typeof(object) && baseType != null)
            {
                RootType = baseType;
                Chain.Add(new TypeTable(baseType));
                baseType = baseType.BaseType;
            }
        }
        /// <summary>
        /// Gets or sets the most derived type in the inheritance chain.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the root (most base) type in the inheritance chain, excluding <see cref="object"/>.
        /// </summary>
        public Type RootType { get; set; }

        /// <summary>
        /// Gets the list of <see cref="TypeTable"/> entries representing the inheritance chain, ordered from most derived to most base.
        /// </summary>
        public List<TypeTable> Chain { get; }

        /// <summary>
        /// Returns a formatted string representation of the inheritance chain, showing table names and their columns indented by depth.
        /// </summary>
        /// <returns>A multi-line string describing the inheritance chain and its columns.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            int tabCount = 0;
            Chain.Each(typeTable =>
            {
                tabCount.Times(num => builder.Append("\t"));
                builder.AppendLine(typeTable.GetTableName());
                typeTable.PropertyColumns.Each(propertyColumn =>
                {
                    builder.Append("\t");
                    tabCount.Times(num => builder.Append("\t-"));
                    builder.AppendLine(propertyColumn.Column.Name);
                });
                tabCount++;
            });

            return builder.ToString();
        }

        /// <summary>
        /// Determines whether the described type extends (inherits from) the specified type.
        /// </summary>
        /// <param name="type">The type to check for in the inheritance chain.</param>
        /// <returns>True if the specified type is in the inheritance chain; otherwise false.</returns>
        public bool Extends(Type type)
        {
            return Chain.FirstOrDefault(tt => tt.Type == type) != null;
        }
    }
}
