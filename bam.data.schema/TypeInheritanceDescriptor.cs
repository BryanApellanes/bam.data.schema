using Bam.Net;
using Bam.Net.Data.Repositories;
using System.Text;

namespace Bam.Data.Schema
{
    public class TypeInheritanceDescriptor
    {
        public TypeInheritanceDescriptor() { }
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
        public Type Type { get; set; }
        public Type RootType { get; set; }
        public List<TypeTable> Chain { get; }

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

        public bool Extends(Type type)
        {
            return Chain.FirstOrDefault(tt => tt.Type == type) != null;
        }
    }
}
