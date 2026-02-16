namespace Bam.Data.Schema
{
    /// <summary>
    /// A model that wraps an <see cref="IXrefInfo"/> for use in code generation templates, providing a pluralized property name.
    /// </summary>
    public class XrefInfoModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="XrefInfoModel"/> wrapping the specified cross-reference info.
        /// </summary>
        /// <param name="xrefInfo">The cross-reference info to wrap.</param>
        public XrefInfoModel(IXrefInfo xrefInfo)
        {
            Model = xrefInfo;
        }

        /// <summary>
        /// Gets or sets the underlying cross-reference info.
        /// </summary>
        public IXrefInfo Model { get; set; }

        /// <summary>
        /// Gets the pluralized list table name, used as the property name in generated code.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return Model.ListTableName.Pluralize();
            }
        }
    }
}
