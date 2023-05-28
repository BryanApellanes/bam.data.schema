using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bam.Net.Data.Schema
{
    public interface IDaoCodeWriter
    {
        string Namespace { get; set; }
        IDaoTargetStreamResolver DaoTargetStreamResolver { get; set; }
        void WriteContextClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root);
        void WriteDaoClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);
        void WriteQueryClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);
        void WritePagedQueryClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);
        void WriteQiClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);
        void WriteCollectionClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);
        void WriteColumnsClass(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);

        void WritePartial(ISchemaDefinition schema, Func<string, Stream> targetResolver, string root, Table table);
    }
}
