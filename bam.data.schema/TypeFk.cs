/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Reflection;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Foreign key descriptor for generated TypeSchemas, representing a parent-child relationship between CLR types.
    /// </summary>
    public class TypeFk : ITypeFk
	{
		/// <summary>
		/// The type of the Primary Key poco
		/// </summary>
		public Type PrimaryKeyType { get; set; } = null!;

		/// <summary>
		/// The property of the Primary Key poco
		/// that represents the Id/Primary Key
		/// </summary>
		public PropertyInfo PrimaryKeyProperty { get; set; } = null!;

		/// <summary>
		/// The type of the Foreign Key poco
		/// </summary>
		public Type ForeignKeyType { get; set; } = null!;

		/// <summary>
		/// The Foreign Key property that references the
		/// Primary Key
		/// </summary>
		public PropertyInfo ForeignKeyProperty { get; set; } = null!;

		/// <summary>
		/// The property that represents the collection
		/// of Foreign Keys that reference the same
		/// Primary Key
		/// </summary>
		public PropertyInfo CollectionProperty { get; set; } = null!;

		/// <summary>
		/// The property that represents the Parent
		/// Primary Key instance on the Foreign Key
		/// </summary>
		public PropertyInfo ChildParentProperty { get; set; } = null!;

        /// <summary>
        /// Returns a string representation of this foreign key descriptor showing the primary key and foreign key type/property pairs.
        /// </summary>
        /// <returns>A string in the format "PK:Type.Property,FK:Type.Property".</returns>
        public override string ToString()
        {
            return $"PK:{PrimaryKeyType.FullName}.{PrimaryKeyProperty.Name},FK:{ForeignKeyType.FullName}.{ForeignKeyProperty.Name}";
        }
        /// <summary>
        /// Gets the SHA1 hash of this foreign key descriptor's string representation, used for identity comparison.
        /// </summary>
        public string Hash
        {
            get
            {
                return ToString().Sha1();
            }
        }

		/// <summary>
		/// Determines whether the specified object is a <see cref="TypeFk"/> with the same primary key and foreign key types.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>True if the objects have the same primary key and foreign key types; otherwise false.</returns>
		public override bool Equals(object? obj)
		{
			TypeFk? compareTo = obj as TypeFk;
			if (compareTo != null) 
			{
				return PrimaryKeyType.Equals(compareTo.PrimaryKeyType) && ForeignKeyType.Equals(compareTo.ForeignKeyType);
			}
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns a hash code based on the primary key and foreign key types.
		/// </summary>
		/// <returns>A hash code for this foreign key descriptor.</returns>
		public override int GetHashCode()
		{
            return this.GetHashCode(PrimaryKeyType, ForeignKeyType);
		}
    }
}
