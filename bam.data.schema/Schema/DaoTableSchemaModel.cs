namespace Bam.Data.Schema
{
    /// <summary>
    /// A model that represents the data necessary to render a Table into a Dao.
    /// </summary>
    public class DaoTableSchemaModel
    {
        /// <summary>
        /// Gets or sets the table model containing columns and foreign keys.
        /// </summary>
        public ITable Model { get; set; }

        /// <summary>
        /// Gets or sets the parent schema definition.
        /// </summary>
        public IDaoSchemaDefinition Schema { get; set; }

        /// <summary>
        /// Gets or sets the namespace for generated code.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets the camelCased pluralized class name for the table, used in code generation.
        /// </summary>
        public string CamelCasedPluralizedClassName => PluralizedClassName.CamelCase();

        /// <summary>
        /// Gets the pluralized class name for the table.
        /// </summary>
        public string PluralizedClassName => Model.ClassName.Pluralize();

        /// <summary>
        /// Gets the foreign key column models with numeric suffixes appended to reference names for disambiguation.
        /// </summary>
        public ForeignKeyColumnModel[] SuffixedForeignKeys
        {
            get
            {
                int i = 0;
                List<ForeignKeyColumnModel> results = new List<ForeignKeyColumnModel>();
                foreach(ForeignKeyColumn fk in Model.ForeignKeys)
                {
                    ForeignKeyColumn copy = fk.CopyAs<ForeignKeyColumn>();
                    copy.ReferenceNameSuffix = (++i).ToString();
                    results.Add(new ForeignKeyColumnModel(copy, Namespace));
                }
                return results.ToArray();
            }
        }

        private HashSet<string> _foreignKeyNames;
        protected HashSet<string> ForeignKeyNames
        {
            get
            {
                if (_foreignKeyNames == null)
                {
                    _foreignKeyNames = new HashSet<string>();
                    foreach (ForeignKeyColumn fk in Model.ForeignKeys)
                    {
                        _foreignKeyNames.Add(fk.Name);
                    }
                }

                return _foreignKeyNames;
            }
        }
        
        /// <summary>
        /// Gets the columns that are not foreign keys, used in code generation.
        /// </summary>
        public NonForeignKeyColumnModel[] NonForeignKeyColumns
        {
            get
            {
                return Model.Columns.Where(c => !(c is ForeignKeyColumn) && !ForeignKeyNames.Contains(c.Name)).Select(c => new NonForeignKeyColumnModel(c)).ToArray();
            }
        }

        /// <summary>
        /// Gets the foreign keys that reference this table, wrapped as models for code generation.
        /// </summary>
        public ReferencingForeignKeyModel[] ReferencingForeignKeys
        {
            get
            {
                return Model.ReferencingForeignKeys.Select(rfk => new ReferencingForeignKeyModel(rfk)).ToArray();
            }
        }

        /// <summary>
        /// Gets the cross-reference info entries where this table is the left side.
        /// </summary>
        public XrefInfoModel[] LeftXrefs
        {
            get
            {
                return Schema.LeftXrefsFor(Model.Name).Select(x => new XrefInfoModel(x)).ToArray();
            }
        }

        /// <summary>
        /// Gets the cross-reference info entries where this table is the right side.
        /// </summary>
        public XrefInfoModel[] RightXrefs
        {
            get
            {
                return Schema.RightXrefsFor(Model.Name).Select(x => new XrefInfoModel(x)).ToArray();
            }
        }
    }
}
