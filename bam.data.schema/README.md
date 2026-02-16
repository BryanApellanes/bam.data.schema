# bam.data.schema

Database schema definition, extraction, and DAO code generation from CLR types and existing databases.

## Overview

`bam.data.schema` provides the schema layer for the Bam framework, bridging the gap between CLR types and relational database schemas. It defines the data structures that describe database tables, columns, foreign keys, and cross-references, and provides generators that produce DAO (Data Access Object) source code from these schema definitions.

The library operates in two directions. **Forward engineering**: the `SchemaProvider` and `TypeInheritanceSchemaProvider` analyze CLR types to create `TypeSchema` and `DaoSchemaDefinition` objects, which the `DaoGenerator` then renders into C# source files using embedded templates. **Reverse engineering**: various `DaoSchemaExtractor` implementations (for SQL Server, MySQL, PostgreSQL, SQLite, and Firebird) connect to existing databases and extract their schema into `DaoSchemaDefinition` objects, which can then also be used for code generation.

The schema definition model (`DaoSchemaDefinition`) contains tables, columns (with data types, nullability, and key information), foreign key relationships, and cross-reference (many-to-many) junction tables. The `DaoSchemaManager` provides a builder API for constructing and manipulating these definitions, with optional auto-save and schema augmentations (auto-id, audit columns, UUID, CUID).

## Key Classes

| Class | Description |
|---|---|
| `DaoSchemaDefinition` | The central schema model: contains tables, foreign keys, and xref tables. Supports JSON serialization, loading, saving, and merging. |
| `DaoGenerator` | Code generator that writes DAO classes, query classes, collection classes, context classes, and column classes from a `DaoSchemaDefinition` using an `IDaoCodeWriter`. |
| `DaoSchemaExtractor` | Abstract base for extracting schema from an existing database. Subclasses implement database-specific metadata queries. |
| `MsSqlSchemaExtractor` | SQL Server schema extractor. |
| `MySqlSchemaExtractor` | MySQL schema extractor. |
| `NpgsqlSchemaExtractor` / `PotsgresSchemaExtractor` | PostgreSQL schema extractors. |
| `SQLiteSchemaExtractor` | SQLite schema extractor. |
| `FirebirdSqlSchemaExtractor` | (Stub) Firebird schema extractor; all methods throw `NotImplementedException`. |
| `DaoSchemaManager` | Builder for constructing `DaoSchemaDefinition` programmatically: add tables, columns, foreign keys, set key columns. Supports augmentations. |
| `Table` | Represents a database table with columns, a key column, and a connection name. |
| `Column` | Represents a database column with name, data type, nullability, max length, and key designation. |
| `ForeignKeyColumn` | Describes a foreign key relationship between two tables. |
| `XrefTable` | Describes a cross-reference (many-to-many) junction table. |
| `SchemaProvider` | Analyzes CLR types to create `TypeSchema` and `DaoSchemaDefinition` objects, handling foreign keys, xrefs, and type inheritance. |
| `TypeInheritanceSchemaProvider` | Extension of `SchemaProvider` that produces schemas reflecting CLR type inheritance hierarchies as separate tables. |
| `TypeSchema` | Describes CLR types as database-like structures: tables (types), foreign keys, and xrefs with a content-based hash. |
| `TypeSchemaPropertyManager` | Manages property-level operations on `TypeSchema` instances, including parent/child relationship traversal. |
| `SchemaNameMap` | Bidirectional mapping between table/column names and class/property names for schema-to-code translation. |
| `MappedSchemaDefinition` | Applies a `SchemaNameMap` to a `DaoSchemaDefinition` to rename classes and properties. |
| `DataNamespaces` | Utility that derives Dao and Wrapper namespace names from a base namespace. |
| `DaoSchemaDefinitionCreateResult` | Result object containing both `DaoSchemaDefinition` and `TypeSchema` from a schema creation operation. |
| `AutoIdSchemaManager` / `CuidSchemaManager` / `UuidSchemaManager` | Schema augmentations that automatically add Id, Cuid, or Uuid columns to tables. |

## Dependencies

### Project References
- bam.base
- bam.configuration
- bam.data.firebird
- bam.data.mssql
- bam.data.mysql
- bam.data.oracle
- bam.data.postgres
- bam.data

### Package References
- Newtonsoft.Json 13.0.4

### Embedded Resource Templates
- Class.tmpl, Collection.tmpl, ColumnsClass.tmpl, ColumnsProperty.tmpl
- Context.tmpl, ContextMethods.tmpl
- DaoCollectionProperty.tmpl, ForeignKeyProperty.tmpl, KeyProperty.tmpl
- PagedQueryClass.tmpl, Partial.tmpl, Property.tmpl
- QiClass.tmpl, QueryClass.tmpl, XrefProperty.tmpl
- ChildDaoCollectionAdd.tmpl, ChildXrefCollectionAdd.tmpl

### Target Framework
- net10.0

## Usage Examples

### Creating a schema from CLR types
```csharp
var schemaProvider = new SchemaProvider();
var types = new[] { typeof(Customer), typeof(Order) };

DaoSchemaDefinitionCreateResult result = schemaProvider.CreateDaoSchemaDefinition(types);
DaoSchemaDefinition schema = result.DaoSchemaDefinition;
TypeSchema typeSchema = result.TypeSchema;
```

### Generating DAO source code
```csharp
var codeWriter = new DaoCodeWriter(); // IDaoCodeWriter implementation
var generator = new DaoGenerator(codeWriter, "MyApp.Data.Dao");

generator.Generate(schema, "./GeneratedSource/");
```

### Extracting schema from an existing database
```csharp
var extractor = new MsSqlSchemaExtractor();
extractor.ConnectionString = "Server=.;Database=MyDb;...";

DaoSchemaDefinition schema = extractor.Extract();

// Optionally generate code from extracted schema
generator.Generate(schema, "./ReverseEngineered/");
```

### Building a schema programmatically
```csharp
var schemaManager = new DaoSchemaManager();
schemaManager.SetSchema("MySchema");
schemaManager.AddTable("Customer", "Customer");
schemaManager.AddColumn("Customer", new Column("Name", DataTypes.String));
schemaManager.AddColumn("Customer", new Column("Email", DataTypes.String));
schemaManager.SetKeyColumn("Customer", "Id");
```

### Using DataNamespaces
```csharp
var ns = new DataNamespaces("MyApp.Data");
// ns.BaseNamespace == "MyApp.Data"
// ns.DaoNamespace == "MyApp.Data.Dao"
// ns.WrapperNamespace == "MyApp.Data.Wrappers"
```

## Known Gaps / Not Yet Implemented

- **`FirebirdSqlSchemaExtractor`**: All 10 abstract methods (`GetSchemaName`, `GetTableNames`, `GetKeyColumnName`, `GetColumnNames`, `GetColumnDataType`, `GetColumnDbDataType`, `GetColumnMaxLength`, `GetColumnNullable`, `GetForeignKeyColumns`, `SetConnectionName`) throw `NotImplementedException`.
- **`TypeSchemaPropertyInfo`**: Most `PropertyInfo`-interface members throw `NotImplementedException`. This appears to be a minimal shim used for specific schema operations rather than a full implementation.
- **`MsSqlSchemaExtractor`**: Has a TODO to update metadata retrieval to use fewer queries.
- **`SchemaProvider`**: Has a TODO to enable more granular manipulation of schema file paths via a `PathProvider` property.
- **`Column`**: Has a TODO to create a presentation-level class that represents the current class.
