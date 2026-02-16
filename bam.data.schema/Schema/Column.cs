/*
	Copyright © Bryan Apellanes 2015  
*/

//using Bam.Presentation.Html;
using System.Text.RegularExpressions;

namespace Bam.Data.Schema
{
    /// <summary>
    /// Represents a database column with name, data type, nullability, and other schema metadata used by the DAO code generator.
    /// </summary>
    public partial class Column : IColumn
    {
        /// <summary>
        /// Initializes a new empty instance of <see cref="Column"/>.
        /// </summary>
        public Column()
        {
        }

        /// <summary>
        /// Instantiate a column where Type = ULong, AllowNull = false, Key = true
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="tableName"></param>
        public Column(string columnName, string tableName)
        {
            this.Name = columnName;
            this.TableName = tableName;
            this.DataType = DataTypes.ULong;
            this.AllowNull = false;
            this.Key = true;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Column"/> with the specified name, data type, nullability, and optional max length.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="dataType">The data type of the column.</param>
        /// <param name="allowNull">Whether the column allows null values.</param>
        /// <param name="maxLength">The maximum length for the column, if applicable.</param>
        public Column(string columnName, DataTypes dataType, bool allowNull = true, string maxLength = "")
        {
            this.Name = columnName;
            this.DataType = dataType;
            this.AllowNull = allowNull;
            this.MaxLength = maxLength;
        }

        internal Column(string tableName)
        {
            this.TableName = tableName;
        }

        /// <summary>
        /// Gets or sets the name of the table this column belongs to.
        /// </summary>
        [Exclude]
        public string TableName { get; set; }

        string _tableClassName;
        /// <summary>
        /// Gets or sets the PascalCase class name derived from the table name.
        /// </summary>
        [Exclude]
        public string TableClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableClassName))
                {

                    string val = TableName;
                    if (!string.IsNullOrEmpty(val))
                    {
                        val = Table.GetClassName(val); //TableName.PascalCase(true, " ", "_").DropLeadingNonLetters();
                    }
                    _tableClassName = val;
                }
                return _tableClassName;
            }
            set => _tableClassName = value;
        }

        string name;
        /// <summary>
        /// Gets or sets the column name, with whitespace automatically removed.
        /// </summary>
        public string Name
        {
            get => name;
            set => this.name = Regex.Replace(value, @"\s", string.Empty);
        }

        string _propertyName;
        /// <summary>
        /// Gets the value of the PropertyName this Column
        /// will be converted to during code generation
        /// </summary>
        public string PropertyName
        {
            get => string.IsNullOrEmpty(_propertyName) ? GetPropertyName(Name) : _propertyName;
            set => _propertyName = GetPropertyName(value);
        }

        /// <summary>
        /// Converts a column name to a valid C# property name using PascalCase and removing non-alphanumeric characters.
        /// </summary>
        /// <param name="name">The column name to convert.</param>
        /// <returns>A valid C# property name derived from the column name.</returns>
        public static string GetPropertyName(string name)
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
        
        /// <summary>
        /// The Dao defined DataType of the column
        /// </summary>
        // TODO: create a presentation level class that represents the current class
        //[DropDown(typeof(DataTypes))] 
        public virtual DataTypes DataType { get; set; }

        /// <summary>
        /// Gets or sets the maximum length for the column, if applicable.
        /// </summary>
        public string MaxLength { get; set; }

        /// <summary>
        /// The string representation of the Dao defined data type 
        /// translated to its native csharp type equivalent.  Used by
        /// generator and not directly refernced in code
        /// </summary>
        public string NativeType
        {
            get
            {
                switch (DataType)
                {
                    case DataTypes.Boolean:
                        return "bool?";
                    case DataTypes.Int:
                        return "int?";
                    case DataTypes.UInt:
                        return "uint?";
                    case DataTypes.Long:
                        return "long?";
                    case DataTypes.ULong:
                        return "ulong?";
                    case DataTypes.Decimal:
                        return "decimal?";
                    case DataTypes.String:
                        return "string";
                    case DataTypes.ByteArray:
                        return "byte[]";
                    case DataTypes.DateTime:
                        return "DateTime?";
                    default:
                        return "string";
                }
            }            
        }

        string _dbDataType;
        /// <summary>
        /// The database equivalent of the DataType
        /// </summary>
        public string DbDataType
        {
            get
            {
                if (string.IsNullOrEmpty(_dbDataType))
                {
                    switch (DataType)
                    {
                        case DataTypes.Default:
                            SetDbDataType("VarChar", "4000");
                            break;
                        case DataTypes.Boolean:
                            SetDbDataType("Bit", "1");
                            break;
                        case DataTypes.Int:
                            SetDbDataType("Int", "10");
                            break;
                        case DataTypes.UInt:
                            SetDbDataType("Int", "10");
                            break;
                        case DataTypes.Long:
                            SetDbDataType("BigInt", "19");
                            break;
                        case DataTypes.ULong:
                            SetDbDataType("BigInt", "19");
                            break;
                        case DataTypes.Decimal:
                            SetDbDataType("Decimal", "28");
                            break;
                        case DataTypes.String:
                            SetDbDataType("VarChar", "4000");
                            break;
                        case DataTypes.ByteArray:
                            SetDbDataType("VarBinary", "8000");
                            break;
                        case DataTypes.DateTime:
                            SetDbDataType("DateTime", "8");
                            break;
                        default:
                            SetDbDataType("VarChar", "4000");
                            break;
                    }
                }

                return _dbDataType;
            }
            set
            {
                _dbDataType = value;
            }
        }

        private void SetDbDataType(string dbDataType, string max = null)
        {
            _dbDataType = dbDataType;
            if (string.IsNullOrEmpty(MaxLength) && !string.IsNullOrEmpty(max))
            {
                MaxLength = max;
            }
        }

        /// <summary>
        /// Gets or sets whether this column allows null values.
        /// </summary>
        public virtual bool AllowNull
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this column is the primary key column.
        /// </summary>
        [Exclude]
        public virtual bool Key
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            return $"{this.TableName}.{this.Name}".ToLowerInvariant().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Column col)
            {
                return col.GetHashCode() == this.GetHashCode();
            }
            else
            {
                return base.Equals(obj);
            }
        }

    }
}
