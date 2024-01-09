/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Bam.Net.Data.Schema
{
    public class DaoSchemaManagerResult : IDaoSchemaManagerResult
    {
        public DaoSchemaManagerResult(string message)
        {
            this.Message = message;
            this.Success = true;
        }

        public DaoSchemaManagerResult(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
        }

        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }

        public bool Success { get; set; }
        public string Namespace { get; set; }
        public string SchemaName { get; set; }

        [Exclude]
        [JsonIgnore]
        [YamlIgnore]
        [XmlIgnore]
		public FileInfo DaoAssembly { get; set; }
    }
}
