using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bam.Data.Schema
{
    public interface IDaoCodeWriter
    {
        string Namespace { get; set; }
        IDaoTargetStreamResolver DaoTargetStreamResolver { get; set; }
        void WriteContextClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root);
        void WriteDaoClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
        void WriteQueryClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
        void WritePagedQueryClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
        void WriteQiClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
        void WriteCollectionClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
        void WriteColumnsClass(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);

        void WritePartial(IDaoSchemaDefinition schema, Func<string, Stream> targetResolver, string root, ITable table);
    }
}
