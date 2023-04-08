using Bam.Net.Data.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Data.Schema
{
    public class DaoSchema
    {
        public static SchemaManagerResult GenerateAssembly(FileInfo dbJs, DirectoryInfo compileTo, DirectoryInfo tempSourceDir)
        {
            SchemaManager schemaManager = new UuidSchemaManager();

            DirectoryInfo partialsDir = new DirectoryInfo(Path.Combine(dbJs.Directory.FullName, "DaoPartials"));
            SchemaManagerResult schemaManagerResult = new SchemaManagerResult("Generator Not Run, invalid file extension", false);
            if (dbJs.Extension.ToLowerInvariant().Equals(".js"))
            {
                schemaManagerResult = schemaManager.GenerateDaoAssembly(dbJs, compileTo, tempSourceDir, partialsDir);
            }
            else if (dbJs.Extension.ToLowerInvariant().Equals(".json"))
            {
                string json = File.ReadAllText(dbJs.FullName);
                schemaManagerResult = schemaManager.GenerateDaoAssembly(json, compileTo, tempSourceDir);
            }

            return schemaManagerResult;
        }
    }
}
