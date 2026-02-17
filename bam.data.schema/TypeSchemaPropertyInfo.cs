/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Reflection;
using Bam.Data.Schema;

namespace Bam.Data.Repositories
{
    /// <summary>
    /// Represents information about a missing primary key/id column or missing foreign key.
    /// </summary>
    /// <seealso cref="System.Reflection.PropertyInfo" />
    public class TypeSchemaPropertyInfo: PropertyInfo
	{
		/// <summary>
		/// Initializes a new instance of <see cref="TypeSchemaPropertyInfo"/> representing a missing key/id property on the specified type.
		/// </summary>
		/// <param name="name">The name of the missing property.</param>
		/// <param name="decrlaringType">The type that should declare this property.</param>
		/// <param name="tableNameProvier">The provider for resolving table names from types.</param>
		public TypeSchemaPropertyInfo(string name, Type decrlaringType, ITypeTableNameProvider tableNameProvier)
		{
			this._name = name;
			this.SetDeclaringType(decrlaringType);
            this.TableNameProvider = tableNameProvier;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="TypeSchemaPropertyInfo"/> representing a missing foreign key property.
		/// </summary>
		/// <param name="name">The name of the missing property.</param>
		/// <param name="declaringType">The type of the primary key table.</param>
		/// <param name="foreignKeyTableType">The type of the foreign key table.</param>
		/// <param name="tableNameProvider">The provider for resolving table names from types.</param>
		public TypeSchemaPropertyInfo(string name, Type declaringType, Type foreignKeyTableType, ITypeTableNameProvider tableNameProvider)
		{
			this._name = name;
			this.SetDeclaringType(declaringType);
			this._foreignKeyTableType = foreignKeyTableType;
            this.TableNameProvider = tableNameProvider;
		}

        protected ITypeTableNameProvider TableNameProvider
        {
            get;
            set;
        }

		/// <inheritdoc />
		public override PropertyAttributes Attributes
		{
			get { throw new NotImplementedException(); }
		}

		/// <inheritdoc />
		public override bool CanRead
		{
			get { throw new NotImplementedException(); }
		}

		/// <inheritdoc />
		public override bool CanWrite
		{
			get { throw new NotImplementedException(); }
		}

		/// <inheritdoc />
		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override MethodInfo? GetGetMethod(bool nonPublic)
		{
			return null;
		}

		/// <inheritdoc />
		public override System.Reflection.ParameterInfo[] GetIndexParameters()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, System.Globalization.CultureInfo? culture)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override Type PropertyType
		{
			get { throw new NotImplementedException(); }
		}

		/// <inheritdoc />
		public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, System.Globalization.CultureInfo? culture)
		{
			throw new NotImplementedException();
		}

		Type _declaringType = null!;
		/// <inheritdoc />
		public override Type DeclaringType
		{
			get { return _declaringType; }
		}

		/// <inheritdoc />
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		string _name;
		/// <inheritdoc />
		public override string Name
		{
			get { return _name; }
		}

		/// <inheritdoc />
		public override Type ReflectedType
		{
			get { throw new NotImplementedException(); }
		}

		
		protected internal void SetDeclaringType(Type type) 
		{
			_declaringType = type;
		}

        /// <summary>
        /// Converts this property info to a <see cref="KeyColumn"/> using the default table name provider.
        /// </summary>
        /// <returns>A key column representing this property.</returns>
        public KeyColumn ToKeyColumn()
        {
            return ToKeyColumn(TableNameProvider);
        }

		/// <summary>
		/// Converts this property info to a <see cref="KeyColumn"/> using the specified table name provider.
		/// </summary>
		/// <param name="tableNameProvider">The provider for resolving table names from types.</param>
		/// <returns>A key column representing this property.</returns>
		public KeyColumn ToKeyColumn(ITypeTableNameProvider tableNameProvider) {
			string name = "Id";
			if (DeclaringType != null) 
			{
				PropertyInfo keyProperty = SchemaProvider.GetKeyProperty(DeclaringType);
				if (keyProperty != null) 
				{
					name = keyProperty.Name;
				}
			}
			return new KeyColumn 
			{
				TableName = tableNameProvider.GetTableName(DeclaringType!),
				Name = name,
				DataType = DataTypes.ULong
			};
		}

		Type _foreignKeyTableType = null!;
		/// <summary>
		/// Converts this property info to a <see cref="ForeignKeyColumn"/> using the specified table name provider.
		/// </summary>
		/// <param name="tableNameProvider">The provider for resolving table names from types; defaults to <see cref="EchoTypeTableNameProvider"/> if null.</param>
		/// <returns>A foreign key column representing this property.</returns>
		public ForeignKeyColumn ToForeignKeyColumn(ITypeTableNameProvider? tableNameProvider = null) {
            ForeignKeyColumn result = new ForeignKeyColumn(Name, SchemaProvider.GetTableNameForType(_foreignKeyTableType),
                SchemaProvider.GetTableNameForType(DeclaringType, tableNameProvider))
            {
                DataType = DataTypes.ULong
            };

            return result;
		}
	}
}
