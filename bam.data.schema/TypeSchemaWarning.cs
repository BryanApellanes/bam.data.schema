using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Represents a warning generated during type schema creation, such as a missing key property or referencing property.
    /// </summary>
    public class TypeSchemaWarning : ITypeSchemaWarning
    {
        /// <summary>
        /// Gets or sets the type of warning.
        /// </summary>
        public TypeSchemaWarnings Warning { get; set; }

        /// <summary>
        /// Gets or sets the parent type associated with this warning.
        /// </summary>
        public Type ParentType { get; set; }

        /// <summary>
        /// Gets or sets the foreign key type associated with this warning, if applicable.
        /// </summary>
        public Type ForeignKeyType { get; set; }

        public override string ToString()
        {
            string fkString = ForeignKeyType != null ? $", ForeignKeyType={ForeignKeyType?.Name}" : "";
            return $"{Warning.ToString()}: ParentType={ParentType?.Name ?? "null"}" + fkString;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeSchemaWarning typeSchemaWarning)
            {
                return typeSchemaWarning.Warning == Warning && typeSchemaWarning.ParentType == ParentType &&
                       typeSchemaWarning.ForeignKeyType == ForeignKeyType;
            }

            return false;
        }

        /// <summary>
        /// Creates a <see cref="TypeSchemaWarning"/> from the specified event args.
        /// </summary>
        /// <param name="args">The event args containing the warning details.</param>
        /// <returns>A new <see cref="ITypeSchemaWarning"/> instance.</returns>
        public static ITypeSchemaWarning FromEventArgs(TypeSchemaWarningEventArgs args)
        {
            return new TypeSchemaWarning
            {
                Warning = args.Warning,
                ParentType =  args.ParentType,
                ForeignKeyType = args.ForeignKeyType
            };
        }
        
        /// <summary>
        /// Converts this warning to a <see cref="TypeSchemaWarningEventArgs"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TypeSchemaWarningEventArgs"/> instance representing this warning.</returns>
        public TypeSchemaWarningEventArgs ToEventArgs()
        {
            return TypeSchemaWarningEventArgs.FromTypeSchemaWarning(this);
        }
    }
}