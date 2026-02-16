/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Xml.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Represents the result of a schema manager operation, including success status, messages, and optionally the generated DAO assembly.
    /// </summary>
    public class DaoSchemaManagerResult : IDaoSchemaManagerResult
    {
        /// <summary>
        /// Initializes a new successful result with the specified message.
        /// </summary>
        /// <param name="message">The result message.</param>
        public DaoSchemaManagerResult(string message)
        {
            this.Message = message;
            this.Success = true;
        }

        /// <summary>
        /// Initializes a new result with the specified message and success status.
        /// </summary>
        /// <param name="message">The result message.</param>
        /// <param name="success">Whether the operation was successful.</param>
        public DaoSchemaManagerResult(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
        }

        /// <summary>
        /// Gets or sets the result message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception message if an error occurred.
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets the stack trace if an error occurred.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the generated code.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the file info of the generated DAO assembly, if applicable.
        /// </summary>
        [Exclude]
        [JsonIgnore]
        [YamlIgnore]
        [XmlIgnore]
		public FileInfo DaoAssembly { get; set; }
    }
}
