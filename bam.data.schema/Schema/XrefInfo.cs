/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    public sealed partial class XrefInfo : IXrefInfo
    {
        public XrefInfo(string parentTableName, string xrefTableName, string listTableName)
        {
            this.ParentTableName = parentTableName;
            this.XrefTableName = xrefTableName;
            this.ListTableName = listTableName;
        }

        public string ParentTableName { get; set; }
        public string XrefTableName { get; set; }
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
