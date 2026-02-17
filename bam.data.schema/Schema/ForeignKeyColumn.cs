/*
	Copyright © Bryan Apellanes 2015  
*/

namespace Bam.Data.Schema
{
    /// <summary>
    /// A column that represents a foreign key
    /// </summary>
    public partial class ForeignKeyColumn: Column, IForeignKeyColumn
    {
        /// <summary>
        /// Empty constructor provided for deserialization
        /// </summary>
        public ForeignKeyColumn()
        {
            this.ReferencedTable = string.Empty;
            this.DbDataType = string.Empty;
        }
        
        /// <summary>
        /// Instantiate a new ForeignKeyColumn based on the specified column
        /// referencing the specified referencedTable
        /// </summary>
        /// <param name="column"></param>
        /// <param name="referencedTable"></param>
        public ForeignKeyColumn(IColumn column, string referencedTable)
            : base(column.TableName)
        {
            this.AllowNull = column.AllowNull;
            this.Key = column.Key;
            this.Name = column.Name;
            this.DataType = column.DataType;
            this.ReferencedTable = referencedTable;
            this.DbDataType = column.DbDataType;
        }

        /// <summary>
        /// Instantiate a new ForeignKeyColumn with the specified name
        /// for the specified tableName referencing the specified referencedTable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tableName"></param>
        /// <param name="referencedTable"></param>
        public ForeignKeyColumn(string name, string tableName, string referencedTable)
            : this(new Column(name, tableName), referencedTable)
        {
        }

        /// <summary>
        /// Gets the data type of this foreign key column. Always returns <see cref="DataTypes.ULong"/>.
        /// </summary>
        public override DataTypes DataType
        {
            get => DataTypes.ULong;
            set
            {
                // always ULong
            }
        }

        string referenceName = null!;
        /// <summary>
        /// Gets or sets the foreign key constraint name. Defaults to "FK_{TableName}_{ReferencedTable}" if not explicitly set.
        /// </summary>
        public string ReferenceName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(referenceName))
                {
                    return $"FK_{this.TableName}_{ReferencedTable}";
                }
                else
                {
                    return referenceName;
                }
            }
            set => referenceName = value;
        }

        /// <summary>
        /// Gets or sets an optional suffix appended to the reference name for disambiguation.
        /// </summary>
        public string ReferenceNameSuffix
        {
            get;
            set;
        } = null!;

        /// <summary>
        /// Gets or sets the name of the key column in the referenced (primary) table.
        /// </summary>
        public string ReferencedKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the referenced (primary) table.
        /// </summary>
        public string ReferencedTable { get; set; }

        string _referencedClass = null!;
        /// <summary>
        /// Gets or sets the C# class name of the referenced (primary) table.
        /// </summary>
        public string ReferencedClass
        {
            get => string.IsNullOrEmpty(_referencedClass) ? ReferencedTable.PascalCase(true, " ", "_").TrimNonLetters() : _referencedClass;
            set => _referencedClass = value;
        }

        string _referencingClass = null!;
        /// <summary>
        /// Gets or sets the C# class name of the referencing (foreign key) table.
        /// </summary>
        public string ReferencingClass
        {
            get => string.IsNullOrEmpty(_referencingClass) ? TableName.PascalCase(true, " ", "_").TrimNonLetters() : _referencingClass;
            set => _referencingClass = value;
        }

        /// <summary>
        /// Returns the reference name of this foreign key column.
        /// </summary>
        /// <returns>The foreign key constraint reference name.</returns>
        public override string ToString()
        {
            return this.ReferenceName;
        }

    }
}
