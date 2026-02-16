/*
	Copyright © Bryan Apellanes 2015  
*/
using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Class that provides database schema like relationships
    /// for CLR types.  This class should not be instantiated
    /// directly, instead see <see cref="Bam.Data.Schema.SchemaProvider"/>
    /// </summary>
    public class TypeSchema : ITypeSchema
	{
        /// <summary>
        /// Initializes a new empty instance of <see cref="TypeSchema"/>.
        /// </summary>
        public TypeSchema() { }

        /// <summary>
        /// Gets or sets the set of warnings generated during schema creation.
        /// </summary>
        public HashSet<ITypeSchemaWarning> Warnings { get; set; }

		/// <summary>
		/// Gets or sets the set of CLR types that map to database tables.
		/// </summary>
		public HashSet<Type> Tables { get; set; }

		/// <summary>
		/// Gets or sets the set of foreign key relationships between types.
		/// </summary>
		public HashSet<ITypeFk> ForeignKeys { get; set; }

		/// <summary>
		/// Gets or sets the set of many-to-many cross-reference relationships between types.
		/// </summary>
		public HashSet<ITypeXref> Xrefs { get; set; }

		/// <summary>
		/// Gets or sets how to treat properties whose type is not explicitly supported.
		/// </summary>
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
        /// <summary>
        /// Gets or sets the name of this TypeSchema. Defaults to the <see cref="Hash"/> value if not explicitly set.
        /// </summary>
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
