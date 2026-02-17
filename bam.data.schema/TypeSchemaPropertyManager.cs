using System.Collections;
using System.Reflection;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Manages property operations on objects within a TypeSchema, including saving child collections and setting parent references on child objects.
    /// </summary>
    public class TypeSchemaPropertyManager
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TypeSchemaPropertyManager"/> for the specified type schema.
        /// </summary>
        /// <param name="typeSchema">The type schema describing the type relationships.</param>
        public TypeSchemaPropertyManager(TypeSchema typeSchema)
        {
            TypeSchema = typeSchema;
        }

        /// <summary>
        /// Gets or sets the type schema that defines the foreign key and cross-reference relationships.
        /// </summary>
        public TypeSchema TypeSchema { get; set; }

        /// <summary>
        /// Iterates over each child collection of the parent object, sets UUIDs and parent references on each child, and invokes the writer action.
        /// </summary>
        /// <param name="parent">The parent object whose child collections to save.</param>
        /// <param name="childWriter">An action that receives the child type and child instance for persistence.</param>
        public void SaveCollections(object parent, Action<Type, object> childWriter)
        {
            ForEachChildCollection(parent, (type, coll) =>
            {
                foreach(object child in coll)
                {
                    Meta.SetUuid(child);
                    SetParentProperties(parent, child);
                    childWriter(type, child);
                }
            });
        }
        /// <summary>
        /// Iterates over each child collection property of the parent that corresponds to a foreign key relationship, invoking the specified action for each non-null collection.
        /// </summary>
        /// <param name="parent">The parent object whose child collections to enumerate.</param>
        /// <param name="forEachCollection">An action receiving the child element type and the collection.</param>
        public void ForEachChildCollection(object parent, Action<Type, IEnumerable> forEachCollection)
        {
            Type parentType = parent.GetType();
            List<ITypeFk> fkDescriptors = TypeSchema.ForeignKeys.Where(tfk => tfk.PrimaryKeyType == parentType).ToList();
            foreach (TypeFk fk in fkDescriptors)
            {
                IEnumerable collection = (IEnumerable)fk.CollectionProperty.GetValue(parent)!;
                if (collection != null)
                {
                    forEachCollection(fk.CollectionProperty.GetEnumerableType()!, collection);
                }
            }
        }
        /// <summary>
        /// Sets the foreign key property and parent instance property on the child object to reference the parent.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="child">The child object whose foreign key and parent reference properties to set.</param>
        public void SetParentProperties(object parent, object child)
        {
            Type parentType = parent.GetType();
            Type childType = child.GetType();
            ulong parentId = Meta.GetId(parent)!.Value;
            foreach (TypeFk typeFk in TypeSchema.ForeignKeys.Where(fk => fk.ForeignKeyType == childType && fk.PrimaryKeyType == parentType))
            {
                typeFk.ForeignKeyProperty.SetValue(child, parentId);
                PropertyInfo? parentInstanceProperty = childType.GetProperty(typeFk.PrimaryKeyType.Name);
                if (parentInstanceProperty != null)
                {
                    parentInstanceProperty.SetValue(child, parent);
                }
            }
        }
    }
}
