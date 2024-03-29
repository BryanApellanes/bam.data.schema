/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Data.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Data.Repositories
{
    /// <summary>
    /// Class that provides database schema like relationships
    /// for CLR types.  This class should not be instantiated
    /// directly, instead see <see cref="Bam.Data.Schema.SchemaProvider"/>
    /// </summary>
    public class TypeSchema : ITypeSchema
	{
        public TypeSchema() { }
        public HashSet<ITypeSchemaWarning> Warnings { get; set; }
		public HashSet<Type> Tables { get; set; }
		public HashSet<ITypeFk> ForeignKeys { get; set; }
		public HashSet<ITypeXref> Xrefs { get; set; }

		public DefaultDataTypeBehaviors DefaultDataTypeBehavior { get; set; }

        public override string ToString()
        {
            List<Type> sortedTables = Tables.ToList();
            sortedTables.Sort((t1, t2) => (t1.FullName ?? string.Empty).CompareTo(t2.FullName));
            List<ITypeFk> sortedForeignKeys = ForeignKeys.ToList();
            sortedForeignKeys.Sort((f1, f2) => f1.Hash.CompareTo(f2.Hash));
            List<ITypeXref> sortedXrefs = Xrefs.ToList();
            sortedXrefs.Sort((x1, x2) => x1.Hash.CompareTo(x2.Hash));
            string tables = sortedTables.ToInfoString();
            string tablesInfo = Tables.ToInfoHash();
            string foreignKeyHashes = string.Join("\r\n\t", sortedForeignKeys.Select(fk => fk.Hash).ToArray());
            string xrefHashes = string.Join("\r\n\t", sortedXrefs.Select(x => x.Hash).ToArray());
            return $"{tables}\r\n{tablesInfo}\r\nFKHashes:\r\n\t{foreignKeyHashes}\r\nXrefHashes:\r\n\t{xrefHashes}";
        }
        
        /// <summary>
        /// The sha1 of this TypeSchema.
        /// </summary>
        public string Hash => ToString().Sha1();

        string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    _name = Hash;
                }
                return _name;
            }
            set => _name = value;
        }
	}
}
