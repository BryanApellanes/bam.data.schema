/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Text.RegularExpressions;

namespace Bam.Data.Schema
{
    /// <summary>
    /// A database Table
    /// </summary>
    public partial class Table : ITable
    {
        readonly Dictionary<string, IColumn> _columns;
        List<IForeignKeyColumn> _referencingForeignKeys;
        readonly Dictionary<string, IForeignKeyColumn> _foreignKeys;

        /// <summary>
        /// Initializes a new empty instance of <see cref="Table"/>.
        /// </summary>
        public Table()
        {
            this._columns = new Dictionary<string, IColumn>();
            this._referencingForeignKeys = new List<IForeignKeyColumn>();
            this._foreignKeys = new Dictionary<string, IForeignKeyColumn>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Table"/> with the specified name.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public Table(string tableName)
            : this()
        {
            this.Name = tableName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Table"/> with the specified name and connection name.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="connectionName">The connection name (schema name) for this table.</param>
        public Table(string tableName, string connectionName)
            : this(tableName)
        {
            this.ConnectionName = connectionName;
        }

        /// <summary>
        /// Gets or sets the connection/schema name for this table.
        /// </summary>
        [Exclude]
        public string ConnectionName { get; set; } = null!;

        /// <summary>
        /// Sets the C# property name for the specified column.
        /// </summary>
        /// <param name="columnName">The column name to set the property name for.</param>
        /// <param name="propertyName">The C# property name to assign.</param>
        public void SetPropertyName(string columnName, string propertyName)
        {
            List<IColumn> columns = new List<IColumn>(Columns);
            IColumn? toSet = columns.FirstOrDefault(c => c.Name.Equals(columnName));
            if (toSet != null)
            {
                toSet.PropertyName = propertyName;
                Columns = columns.ToArray();
            }
            else
            {
                List<IForeignKeyColumn> fks = new List<IForeignKeyColumn>(ForeignKeys);
                IForeignKeyColumn? toSetFk = fks.FirstOrDefault(c => c.Name.Equals(columnName));
                if (toSetFk != null)
                {
                    toSetFk.PropertyName = propertyName;
                    ForeignKeys = fks.ToArray();                    
                }
            }
        }

        /// <summary>
        /// Gets the C# property name for the specified column.
        /// </summary>
        /// <param name="columnName">The column name to look up.</param>
        /// <returns>The property name for the column.</returns>
        public string GetPropertyName(string columnName)
        {
            return this[columnName].PropertyName;
        }

        string name = null!;
        /// <summary>
        /// Gets or sets the table name, with whitespace automatically removed.
        /// </summary>
        public string Name
        {
            get => name;
            set => this.name = Regex.Replace(value, @"\s", string.Empty);
        }

        string _className = null!;
        /// <summary>
        /// Gets or sets the C# class name for this table. Defaults to the table name formatted as PascalCase.
        /// </summary>
        public string ClassName
        {
            get => string.IsNullOrEmpty(_className) ? GetClassName(Name) : _className;
            set => _className = value;
        }

        /// <summary>
        /// Gets or sets the columns defined on this table.
        /// </summary>
        public IColumn[] Columns
        {
            get
            {
                lock (_columnLock)
                {
                    return _columns.Values.ToArray();
                }
            }
            set
            {
                lock (_columnLock)
                {
                    _columns.Clear();
                    foreach (Column val in value)
                    {
                        _columns.Add(val.Name, val);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreign key columns defined on this table.
        /// </summary>
        public IForeignKeyColumn[] ForeignKeys
        {
            get
            {
                lock (_columnLock)
                {
                    return _foreignKeys.Values.ToArray();
                }
            }
            set
            {
                lock (_columnLock)
                {
                    _foreignKeys.Clear();
                    foreach (ForeignKeyColumn fk in value)
                    {
                        _foreignKeys.Add(fk.Name, fk);
                    }
                }
            }
        }

        /// <summary>
        /// All ForeignKeyColumns where the current table is referenced.
        /// </summary>
        public IForeignKeyColumn[] ReferencingForeignKeys
        {
            get
            {
                lock (_columnLock)
                {
                    return _referencingForeignKeys.ToArray();
                }
            }
            set
            {
                lock (_columnLock)
                {
                    _referencingForeignKeys = new List<IForeignKeyColumn>(value);
                }
            }
        }
        
        /// <summary>
        /// Gets the primary key column for this table. Returns a default "Id" key column if none is defined.
        /// </summary>
        [Exclude]
        public IColumn Key
        {
            get
            {
                IColumn? key = (from col in Columns
                        where (col is KeyColumn || col.Key)
                        select col).FirstOrDefault();
                
                if (key == null)
                {
                    key = KeyColumn.Default;
                }

                return key;
            }
        }

        /// <summary>
        /// Designates the specified column as the primary key column for this table.
        /// </summary>
        /// <param name="columnName">The name of the column to set as the key.</param>
        public void SetKeyColumn(string columnName)
        {
            IColumn? c = (from cl in Columns
                        where cl.Key
                        select cl).FirstOrDefault();
            if (c != null)
            {
                UnsetKeyColumn(c.Name);
            }

            IColumn col = this[columnName];
            this._columns.Remove(col.Name);
            this.AddColumn(new KeyColumn(col));
        }

        /// <summary>
        /// Converts the specified column to a foreign key column referencing the specified table.
        /// </summary>
        /// <param name="columnName">The name of the column to convert.</param>
        /// <param name="referencedColumn">The key column name in the referenced table.</param>
        /// <param name="referencedTable">The name of the referenced (primary) table.</param>
        public void SetForeignKeyColumn(string columnName, string referencedColumn, string referencedTable)
        {
            IColumn? c = (from cl in Columns
                        where cl.Name.Equals(columnName)
                        select cl).FirstOrDefault();
            if (c != null)
            {
                RemoveColumn(c);
            }
            this.AddColumn(new ForeignKeyColumn(c!, referencedTable));
        }
        
        private void UnsetKeyColumn(string columnName)
        {
            IColumn col = this[columnName];            
            RemoveColumn(col.Name);
            this.AddColumn(new Column { 
                AllowNull = col.AllowNull, 
                Name = col.Name, 
                TableName = col.TableName, 
                DataType = col.DataType });
        }

        /// <summary>
        /// Adds a column with the specified name, data type, and nullability to this table.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="type">The data type of the column.</param>
        /// <param name="allowNull">Whether the column allows null values.</param>
        public void AddColumn(string columnName, DataTypes type, bool allowNull = true)
        {
            AddColumn(new Column { AllowNull = allowNull, Key = false, Name = columnName, TableName = this.Name, DataType = type });
        }

        object _columnLock = new object();
        /// <summary>
        /// Adds the specified column to this table. Foreign key columns are also tracked in the foreign keys collection.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(IColumn column)
        {
            lock (_columnLock)
            {
                column.TableName = this.Name;
                if (column is ForeignKeyColumn fk)
                {
                    if (fk.ReferencedTable.Equals(this.Name))
                    {
                        this._referencingForeignKeys.Add(fk);
                    }

                    if (fk.TableName.Equals(this.Name) && !this._foreignKeys.ContainsKey(fk.Name))
                    {
                        this._foreignKeys.Add(fk.Name, fk);
                    }
                }

                if (!this._columns.ContainsKey(column.Name))
                {
                    this._columns.Add(column.Name, column);
                }
            }
        }
        
        /// <summary>
        /// Removes the specified column from this table.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        public void RemoveColumn(IColumn column)
        {
            RemoveColumn(column.Name);
        }

        /// <summary>
        /// Removes the column with the specified name from this table.
        /// </summary>
        /// <param name="columnName">The name of the column to remove.</param>
        public void RemoveColumn(string columnName)
        {
            lock (_columnLock)
            {
                if (this._columns.ContainsKey(columnName))
                {
                    this._columns.Remove(columnName);
                }
            }
        }

        /// <summary>
        /// Gets the column with the specified name from either the columns or foreign keys collection.
        /// </summary>
        /// <param name="columnName">The name of the column to retrieve.</param>
        /// <returns>The column if found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the column is not found on this table.</exception>
        [Exclude]
        public IColumn this[string columnName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(columnName))
                {
                    throw new ArgumentNullException(nameof(columnName));
                }

                if (this._columns.ContainsKey(columnName))
                {
                    return this._columns[columnName];
                }
                else if (this._foreignKeys.ContainsKey(columnName))
                {
                    return this._foreignKeys[columnName];
                }
                else
                {
                    throw new InvalidOperationException($"The specified column {columnName} was not found on the table {this.Name}");
                }
            }
        }

        /// <summary>
        /// Determines whether this table has a column with the specified name.
        /// </summary>
        /// <param name="columnName">The column name to check for.</param>
        /// <returns>True if the column exists; otherwise false.</returns>
        public bool HasColumn(string columnName)
        {
            return HasColumn(columnName, out IColumn ignore);
        }

        /// <summary>
        /// Determines whether this table has a column with the specified name, and outputs the column if found.
        /// </summary>
        /// <param name="columnName">The column name to check for.</param>
        /// <param name="column">When this method returns, contains the column if found.</param>
        /// <returns>True if the column exists; otherwise false.</returns>
        public bool HasColumn(string columnName, out IColumn column)
        {
            bool result = _columns.ContainsKey(columnName);
            column = _columns[columnName]!;
            return result;
        }
        
        /// <summary>
        /// Returns a string representation of this table, showing the name and class name.
        /// </summary>
        /// <returns>A string in the format "Table.Name=name::Table.ClassName=className".</returns>
        public override string ToString()
        {
            return string.Format("{0}.Name={1}::{0}.ClassName={2}", typeof(Table).Name, this.Name, this.ClassName);
        }

        /// <summary>
        /// Converts a table name to a valid C# class name using PascalCase and removing non-alphanumeric characters.
        /// </summary>
        /// <param name="name">The table name to convert.</param>
        /// <returns>A valid C# class name derived from the table name.</returns>
        public static string GetClassName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            if (name[0].IsNumber())
            {
                name = $"_{name.PascalCase(true, " ", "_").AlphaNumericOnly()}";
                return name;
            }
            return name.PascalCase(true, " ", "_").AlphaNumericOnly();
        }
    }
}
