using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.Data.Schema
{
    public class XrefInfoModel
    {
        public XrefInfoModel(IXrefInfo xrefInfo)
        {
            Model = xrefInfo;
        }

        public IXrefInfo Model { get; set; }

        public string PropertyName
        {
            get
            {
                return Model.ListTableName.Pluralize();
            }
        }
    }
}
