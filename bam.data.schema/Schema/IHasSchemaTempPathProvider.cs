namespace Bam.Data.Schema
{
    public interface IHasSchemaTempPathProvider
    {
        Func<IDaoSchemaDefinition, string> SchemaTempPathProvider { get; set; }
    }
}
