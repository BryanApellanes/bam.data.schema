/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Represents cross-reference (many-to-many) information linking a parent table to a list table through an xref join table.
    /// </summary>
    public sealed partial class XrefInfo : IXrefInfo
    {
        /// <summary>
        /// Initializes a new instance of <see cref="XrefInfo"/> with the specified parent, xref, and list table names.
        /// </summary>
        /// <param name="parentTableName">The name of the parent (current) table.</param>
        /// <param name="xrefTableName">The name of the cross-reference join table.</param>
        /// <param name="listTableName">The name of the related table on the other side of the relationship.</param>
        public XrefInfo(string parentTableName, string xrefTableName, string listTableName)
        {
            this.ParentTableName = parentTableName;
            this.XrefTableName = xrefTableName;
            this.ListTableName = listTableName;
        }

        /// <summary>
        /// Gets or sets the name of the parent table.
        /// </summary>
        public string ParentTableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the cross-reference join table.
        /// </summary>
        public string XrefTableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the related list table.
        /// </summary>
        public string ListTableName { get; set; }
        
/*        public string RenderXrefProperty()
        {
            return Render("XrefProperty.tmpl");
        }

        public string RenderAddToChildDaoCollection()
        {
            return Render("ChildXrefCollectionAdd.tmpl");
        }*/

    }
}
