using System;
using System.IO;

namespace Bam.Data.Schema
{
    public class FsDaoTargetStreamResolver: IDaoTargetStreamResolver
    {
        public Stream GetTargetContextStream(Func<string, Stream> targetResolver, string root, IDaoSchemaDefinition schema)
        {
            string parameterValue = $"{schema.Name}Context";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = table.ClassName;
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetQueryClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}Query";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetPagedQueryClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}PagedQuery";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetQiClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"Qi/{table.ClassName}";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetCollectionStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}Collection";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetColumnsClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            string parameterValue = $"{table.ClassName}Columns";
            return GetTargetStream(targetResolver, root, parameterValue);
        }

        public Stream GetTargetPartialClassStream(Func<string, Stream> targetResolver, string root, ITable table)
        {
            if(targetResolver != null)
            {
                return targetResolver($"{table.Name}_Partial");
            }
            string path = Path.Combine(root, "Partials", $"{table.Name}.cs");
            FileInfo f = new FileInfo(path);
            if (!f.Directory.Exists)
            {
                f.Directory.Create();
            }
            return f.OpenWrite();            
        }

        public Stream GetTargetStream(Func<string, Stream>? targetResolver, string root, string parameterValue)
        {
            Stream stream;
            if (targetResolver != null)
            {
                stream = targetResolver(parameterValue);
            }
            else
            {
                string path = Path.Combine(root, $"{parameterValue}.cs");
                FileInfo file = new FileInfo(path);
                if (file.Directory != null && file.Directory.Exists == false)
                {
                    file.Directory.Create();
                }
                stream = file.OpenWrite();
            }
            return stream;
        }
    }
}
