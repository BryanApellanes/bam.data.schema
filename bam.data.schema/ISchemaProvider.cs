/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Data.Schema;
using Bam.Net.Data.Schema;
using Bam.Net.Logging;

namespace Bam.Net.Data.Repositories
{
    public interface ISchemaProvider: ILoggable
    {
        bool AddAuditFields { get; set; }
        bool AddIdField { get; set; }
        DefaultDataTypeBehaviors DefaultDataTypeBehavior { get; set; }
        bool IncludeCreatedBy { get; set; }
        bool IncludeModifiedBy { get; set; }
        string Instant { get; }
        string Message { get; set; }
        SchemaManager SchemaManager { get; set; }
        string SchemaName { get; set; }
        ITypeTableNameProvider TableNameProvider { get; set; }
        IEnumerable<Type> Types { get; set; }
        Func<IDaoSchemaDefinition, ITypeSchema, string>? TypeSchemaTempPathProvider { get; set; }
        HashSet<ITypeSchemaWarning> TypeSchemaWarnings { get; set; }

        event EventHandler DifferentTypeNamespacesFound;
        event EventHandler ChildParentPropertyNotFound;
        event EventHandler CreatingSchemaStarted;
        event EventHandler CreatingTypeSchemaFinished;
        event EventHandler CreatingTypeSchemaStarted;
        event EventHandler KeyPropertyNotFound;
        event EventHandler ReferencingPropertyNotFound;
        event EventHandler WritingDaoSchemaFinished;
        event EventHandler WritingDaoSchemaStarted;

        DaoSchemaDefinitionCreateResult CreateDaoSchemaDefinition(string schemaName = null);
        DaoSchemaDefinitionCreateResult CreateDaoSchemaDefinition(IEnumerable<Type> types, string schemaName = null);
        TypeSchema CreateTypeSchema();
        TypeSchema CreateTypeSchema(IEnumerable<Type> types, string name = null);
        TypeSchema CreateTypeSchema(string name, params Type[] types);

        DataNamespaces GetDataNamespaces();

        string GetSchemaName(IEnumerable<Type> types);
        string GetSchemaNameOrDefault(string? name = null);
    }
}