/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Reflection;
using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Used to describe a many to many 
    /// relationship between two types.
    /// This would imply that each type
    /// has an IEnumerable property
    /// of the other type
    /// </summary>
    public class TypeXref : ITypeXref
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TypeXref"/> with a default table name provider.
        /// </summary>
        public TypeXref()
        {
            TableNameProvider = new EchoTypeTableNameProvider();
        }
        /// <summary>
        /// Gets or sets the left type in the many-to-many relationship.
        /// </summary>
        public Type Left { get; set; } = null!;

        /// <summary>
        /// Gets or sets the right type in the many-to-many relationship.
        /// </summary>
        public Type Right { get; set; } = null!;

        /// <summary>
        /// The property of the Left type that represents
        /// the collection containing elements of the Right type
        /// </summary>
        public PropertyInfo LeftCollectionProperty
        {
            get;
            set;
        } = null!;

        /// <summary>
        /// Gets or sets the provider used to determine table names from types.
        /// </summary>
        public ITypeTableNameProvider TableNameProvider { get; set; }

        /// <summary>
        /// Gets the DAO table name for the left type.
        /// </summary>
        public string LeftDaoName
        {
            get
            {
                return SchemaProvider.GetTableNameForType(Left, TableNameProvider);
            }
        }

        /// <summary>
        /// Returns a string representation of this cross-reference, showing the left and right type/property pairs.
        /// </summary>
        /// <returns>A string in the format "Left:Type.Property,Right:Type.Property".</returns>
        public override string ToString()
        {
            return $"Left:{Left.FullName}.{LeftCollectionProperty.Name},Right:{Right.FullName}.{RightCollectionProperty.Name}";
        }

        /// <summary>
        /// Gets the SHA1 hash of this cross-reference descriptor's string representation.
        /// </summary>
        public string Hash
        {
            get
            {
                return ToString().Sha1();
            }
        }

        /// <summary>
        /// The property of the Right type that represents 
        /// the collection containing elements of the Left type
        /// </summary>
        public PropertyInfo RightCollectionProperty
        {
            get;
            set;
        } = null!;

        /// <summary>
        /// Gets the DAO table name for the right type.
        /// </summary>
        public string RightDaoName
        {
            get
            {
                return SchemaProvider.GetTableNameForType(Right, TableNameProvider);
            }
        }

        /// <summary>
        /// The name of the LeftCollectionProperty.  Used by underlying 
        /// Poco generator
        /// </summary>
        public string LeftCollectionTypeName
        {
            get
            {
                string value = LeftCollectionProperty.PropertyType.IsArray ? LeftCollectionProperty.PropertyType.FullName! : string.Format("List<{0}.{1}>", Left.Namespace, Left.Name);
                return value;
            }
        }

        /// <summary>
        /// The name of the RightCollectionProperty.  Used by underlying
        /// Poco generator
        /// </summary>
        public string RightCollectionTypeName
        {
            get
            {
                string value = RightCollectionProperty.PropertyType.IsArray ? RightCollectionProperty.PropertyType.FullName! : string.Format("List<{0}.{1}>", Right.Namespace, Right.Name);
                return value;
            }
        }

        /// <summary>
        /// Used by the underlying Poco generator
        /// </summary>
        public string LeftArrayOrList
        {
            get { return LeftCollectionProperty.PropertyType.IsArray ? "Array" : "List"; }
        }

        /// <summary>
        /// Used by the underlying Poco generator
        /// </summary>
        public string RightArrayOrList
        {
            get { return RightCollectionProperty.PropertyType.IsArray ? "Array" : "List"; }
        }

        /// <summary>
        /// Gets "Length" if the right collection is an array, or "Count" if it is a list. Used by the code generator.
        /// </summary>
        public string RightLengthOrCount
        {
            get { return RightCollectionProperty.PropertyType.IsArray ? "Length" : "Count"; }
        }

        /// <summary>
        /// Gets "Length" if the left collection is an array, or "Count" if it is a list. Used by the code generator.
        /// </summary>
        public string LeftLengthOrCount
        {
            get { return LeftCollectionProperty.PropertyType.IsArray ? "Length" : "Count"; }
        }

		/// <summary>
		/// Determines whether the specified object is a <see cref="TypeXref"/> with the same left and right types (in either order).
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>True if the cross-references involve the same two types; otherwise false.</returns>
		public override bool Equals(object? obj)
		{
			TypeXref? compareTo = obj as TypeXref;
			if(compareTo != null)
			{
				return compareTo.Left.Equals(this.Left) && compareTo.Right.Equals(this.Right) ||
					compareTo.Left.Equals(this.Right) && compareTo.Right.Equals(this.Left);
			}
			
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns a hash code based on the sum of the left and right type hash codes.
		/// </summary>
		/// <returns>A hash code for this cross-reference descriptor.</returns>
		public override int GetHashCode()
		{
            return this.Left.GetHashCode() + this.Right.GetHashCode();
        }
    }
}
