namespace Bam.Data.Schema
{
    public interface IDaoTargetStreamResolver
    {
        Stream GetTargetContextStream(Func<string, Stream> targetResolver, string rootDirectory, IDaoSchemaDefinition schema);
        Stream GetTargetClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
        Stream GetTargetQueryClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
        Stream GetTargetPagedQueryClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
        Stream GetTargetQiClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
        Stream GetTargetCollectionStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
        Stream GetTargetColumnsClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);

        Stream GetTargetPartialClassStream(Func<string, Stream> targetResolver, string rootDirectory, ITable table);
    }
}
