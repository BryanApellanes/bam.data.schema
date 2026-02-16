/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// Represents a cross-reference (many-to-many join) table linking two other tables.
    /// </summary>
    public class XrefTable: Table, IXrefTable
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="XrefTable"/>.
        /// </summary>
        public XrefTable()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="XrefTable"/> linking the specified left and right tables.
        /// </summary>
        /// <param name="leftTable">The name of the left table.</param>
        /// <param name="rightTable">The name of the right table.</param>
        public XrefTable(string leftTable, string rightTable)
            : this()
        {
            this.Left = leftTable;
            this.Right = rightTable;
        }

        string _left;
        /// <summary>
        /// Gets or sets the name of the left table. Setting this value updates the xref table name.
        /// </summary>
        public string Left
        {
            get => _left;
            set
            {
                _left = value;
                SetName();
            }
        }

        string _right;
        /// <summary>
        /// Gets or sets the name of the right table. Setting this value updates the xref table name.
        /// </summary>
        public string Right
        {
            get => _right;
            set
            {
                _right = value;
                SetName();
            }
        }

        private void SetName()
        {
            Name = $"{Left}{Right}";
        }
    }
}
