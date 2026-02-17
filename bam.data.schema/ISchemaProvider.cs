/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Data.Schema;
using Bam.Logging;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Defines the contract for creating type schemas and DAO schema definitions from CLR types.
    /// </summary>
    public interface ISchemaProvider: ILoggable
    {
        /// <summary>
        /// Gets or sets whether to add audit fields (Created, Modified) to generated DAO tables.
        /// </summary>
        bool AddAuditFields { get; set; }

        /// <summary>
        /// Gets or sets whether to add an Id key column to generated DAO tables.
        /// </summary>
        bool AddIdField { get; set; }

        /// <summary>
        /// Gets or sets how to treat properties whose type is not explicitly supported.
        /// </summary>
        DefaultDataTypeBehaviors DefaultDataTypeBehavior { get; set; }

        /// <summary>
        /// Gets or sets whether to include a CreatedBy column in the generated DAO tables.
        /// </summary>
        bool IncludeCreatedBy { get; set; }

        /// <summary>
        /// Gets or sets whether to include a ModifiedBy column in the generated DAO tables.
        /// </summary>
        bool IncludeModifiedBy { get; set; }

        /// <summary>
        /// Gets a string representation of the current UTC time.
        /// </summary>
        string Instant { get; }

        /// <summary>
        /// Gets or sets the message associated with the most recent warning event.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the schema manager used for building the DAO schema.
        /// </summary>
        DaoSchemaManager SchemaManager { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema being generated.
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the provider used to determine table names from CLR types.
        /// </summary>
        ITypeTableNameProvider TableNameProvider { get; set; }

        /// <summary>
        /// Gets or sets the collection of CLR types to generate a schema for.
        /// </summary>
        IEnumerable<Type> Types { get; set; }

        /// <summary>
        /// Gets or sets a function that provides a temporary file path for schema generation output.
        /// </summary>
        Func<IDaoSchemaDefinition, ITypeSchema, string> TypeSchemaTempPathProvider { get; set; }

        /// <summary>
        /// Gets or sets the set of warnings generated during type schema creation.
        /// </summary>
        HashSet<ITypeSchemaWarning> TypeSchemaWarnings { get; set; }

        /// <summary>
        /// Occurs when the specified types belong to different namespaces.
        /// </summary>
        event EventHandler DifferentTypeNamespacesFound;

        /// <summary>
        /// Occurs when a child type does not have a property referencing its parent type.
        /// </summary>
        event EventHandler ChildParentPropertyNotFound;

        /// <summary>
        /// Occurs when schema creation begins.
        /// </summary>
        event EventHandler CreatingSchemaStarted;

        /// <summary>
        /// Occurs when type schema creation completes.
        /// </summary>
        event EventHandler CreatingTypeSchemaFinished;

        /// <summary>
        /// Occurs when type schema creation begins.
        /// </summary>
        event EventHandler CreatingTypeSchemaStarted;

        /// <summary>
        /// Occurs when a type has no key property (no KeyAttribute or "Id" property).
        /// </summary>
        event EventHandler KeyPropertyNotFound;

        /// <summary>
        /// Occurs when a child type does not have a foreign key property referencing the parent's key.
        /// </summary>
        event EventHandler ReferencingPropertyNotFound;

        /// <summary>
        /// Occurs when DAO schema writing completes.
        /// </summary>
        event EventHandler WritingDaoSchemaFinished;

        /// <summary>
        /// Occurs when DAO schema writing begins.
        /// </summary>
        event EventHandler WritingDaoSchemaStarted;

        /// <summary>
        /// Creates a DAO schema definition from the types set on this provider.
        /// </summary>
        /// <param name="schemaName">An optional name for the schema.</param>
        /// <returns>The result containing the schema definition and any warnings.</returns>
        DaoSchemaDefinitionCreateResult CreateDaoSchemaDefinition(string? schemaName = null);

        /// <summary>
        /// Creates a DAO schema definition from the specified types.
        /// </summary>
        /// <param name="types">The CLR types to generate a schema for.</param>
        /// <param name="schemaName">An optional name for the schema.</param>
        /// <returns>The result containing the schema definition and any warnings.</returns>
        DaoSchemaDefinitionCreateResult CreateDaoSchemaDefinition(IEnumerable<Type> types, string? schemaName = null);

        /// <summary>
        /// Creates a type schema from the types set on this provider.
        /// </summary>
        /// <returns>A <see cref="TypeSchema"/> describing the relationships between the types.</returns>
        TypeSchema CreateTypeSchema();

        /// <summary>
        /// Creates a type schema from the specified types.
        /// </summary>
        /// <param name="types">The CLR types to analyze.</param>
        /// <param name="name">An optional name for the schema.</param>
        /// <returns>A <see cref="TypeSchema"/> describing the relationships between the types.</returns>
        TypeSchema CreateTypeSchema(IEnumerable<Type> types, string? name = null);

        /// <summary>
        /// Creates a type schema from the specified types with the given name.
        /// </summary>
        /// <param name="name">The name for the schema.</param>
        /// <param name="types">The CLR types to analyze.</param>
        /// <returns>A <see cref="TypeSchema"/> describing the relationships between the types.</returns>
        TypeSchema CreateTypeSchema(string name, params Type[] types);

        /// <summary>
        /// Gets the data namespaces derived from the first type in the <see cref="Types"/> collection.
        /// </summary>
        /// <returns>A <see cref="DataNamespaces"/> instance for the types.</returns>
        DataNamespaces GetDataNamespaces();

        /// <summary>
        /// Gets the schema name derived from the namespace of the first type in the collection.
        /// </summary>
        /// <param name="types">The types to derive the schema name from.</param>
        /// <returns>A schema name based on the first type's namespace.</returns>
        string GetSchemaName(IEnumerable<Type> types);

        /// <summary>
        /// Gets the schema name from the current types, or returns the specified default name.
        /// </summary>
        /// <param name="name">An optional default name to use if no types are set.</param>
        /// <returns>The schema name.</returns>
        string GetSchemaNameOrDefault(string? name = null);
    }
}