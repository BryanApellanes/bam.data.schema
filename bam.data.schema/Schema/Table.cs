/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bam.Net;
using Bam.Net.Data;

namespace Bam.Net.Data.Schema
{
    /// <summary>
    /// A database Table
    /// </summary>
    public partial class Table : ITable
    {
        readonly Dictionary<string, IColumn> _columns;
        List<IForeignKeyColumn> _referencingForeignKeys;
        readonly Dictionary<string, IForeignKeyColumn> _foreignKeys;

        public Table()
        {
            this._columns = new Dictionary<string, IColumn>();
            this._referencingForeignKeys = new List<IForeignKeyColumn>();
            this._foreignKeys = new Dictionary<string, IForeignKeyColumn>();
        }

        public Table(string tableName)
            : this()
        {
            this.Name = tableName;
        }

        public Table(string tableName, string connectionName)
            : this(tableName)
        {
            this.ConnectionName = connectionName;
        }

        [Exclude]
        public string ConnectionName { get; set; }

        public void SetPropertyName(string columnName, string propertyName)
        {
            List<Column> columns = new List<Column>(Columns);
            Column toSet = columns.FirstOrDefault(c => c.Name.Equals(columnName));
            if (toSet != null)
            {
                toSet.PropertyName = propertyName;
                Columns = columns.ToArray();
            }
            else
            {
                List<ForeignKeyColumn> fks = new List<ForeignKeyColumn>(ForeignKeys);
                ForeignKeyColumn toSetFk = fks.FirstOrDefault(c => c.Name.Equals(columnName));
                if (toSetFk != null)
                {
                    toSetFk.PropertyName = propertyName;
                    ForeignKeys = fks.ToArray();                    
                }
            }
        }

        public string GetPropertyName(string columnName)
        {
            return this[columnName].PropertyName;
        }

        string name;
        public string Name
        {
            get => name;
            set => this.name = Regex.Replace(value, @"\s", string.Empty);
        }

        string _className;
        public string ClassName
        {
            get => string.IsNullOrEmpty(_className) ? GetClassName(Name) : _className;
            set => _className = value;
        }

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
                    _referencingForeignKeys = new List<ForeignKeyColumn>(value);
                }
            }
        }
        
        [Exclude]
        public IColumn Key
        {
            get
            {
                Column key = (from col in Columns
                        where (col is KeyColumnModel || col.Key)
                        select col).FirstOrDefault();
                
                if (key == null)
                {
                    key = KeyColumnModel.Default;
                }

                return key;
            }
        }

        public void SetKeyColumn(string columnName)
        {
            Column c = (from cl in Columns
                        where cl.Key
                        select cl).FirstOrDefault();
            if (c != null)
            {
                UnsetKeyColumn(c.Name);
            }

            Column col = this[columnName];
            this._columns.Remove(col.Name);
            this.AddColumn(new KeyColumnModel(col));
        }

        public void SetForeignKeyColumn(string columnName, string referencedColumn, string referencedTable)
        {
            Column c = (from cl in Columns
                        where cl.Name.Equals(columnName)
                        select cl).FirstOrDefault();
            if (c != null)
            {
                RemoveColumn(c);
            }
            this.AddColumn(new ForeignKeyColumn(c, referencedTable));
        }
        
        private void UnsetKeyColumn(string columnName)
        {
            Column col = this[columnName];            
            RemoveColumn(col.Name);
            this.AddColumn(new Column { 
                AllowNull = col.AllowNull, 
                Name = col.Name, 
                TableName = col.TableName, 
                DataType = col.DataType });
        }

        public void AddColumn(string columnName, DataTypes type, bool allowNull = true)
        {
            AddColumn(new Column { AllowNull = allowNull, Key = false, Name = columnName, TableName = this.Name, DataType = type });
        }

        object _columnLock = new object();
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
        
        public void RemoveColumn(IColumn column)
        {
            RemoveColumn(column.Name);
        }

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

        public bool HasColumn(string columnName)
        {
            return HasColumn(columnName, out Column ignore);
        }
        
        public bool HasColumn(string columnName, out IColumn column)
        {
            bool result = _columns.ContainsKey(columnName);
            column = _columns[columnName];
            return result;
        }
        
        public override string ToString()
        {
            return string.Format("{0}.Name={1}::{0}.ClassName={2}", typeof(Table).Name, this.Name, this.ClassName);
        }

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
