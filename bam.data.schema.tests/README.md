# bam.data.schema.tests

Unit tests for the bam.data.schema library.

## Overview

This project contains tests for the database schema definition and management functionality provided by `bam.data.schema`. Tests are written using the Bam test framework with `[UnitTestMenu]` attributes (with a `Selector` of `"dss"` for the main test menu) and the `When.A<T>()` fluent API. The test runner is menu-driven via `BamConsoleContext.StaticMain`.

The tests verify core schema primitives: column creation with expected defaults (data type, nullability, key status), table construction with columns, and `DataNamespaces` derivation from a base namespace.

## Key Classes

| Class | Description |
|---|---|
| `DataSchemaShould` | Main test class containing unit tests for `Column` defaults, `Table` column management, and `DataNamespaces` namespace derivation. |

## Dependencies

### Project References
- bam.base
- bam.console
- bam.shell
- bam.test
- bam.data.schema

### Target Framework
- net10.0 (Exe)

## Running Tests

```bash
dotnet run --project bam.data.schema.tests.csproj -- --ut
```

**Important**: Use `--ut` (not `/ut`) when running from Git Bash, as Git Bash rewrites `/ut` to a filesystem path.

## Usage Examples

### Test: Column with expected defaults
```csharp
Column column = new Column("Id", "TestTable");
// column.Name == "Id"
// column.TableName == "TestTable"
// column.DataType == DataTypes.ULong
// column.AllowNull == false
// column.Key == true
```

### Test: Table with columns
```csharp
Table table = new Table("TestTable");
table.AddColumn("Name", DataTypes.String);
table.AddColumn("Age", DataTypes.Int);
// table.Columns.Length == 2
// table.HasColumn("Name") == true
```

### Test: DataNamespaces derivation
```csharp
DataNamespaces ns = new DataNamespaces("MyApp.Data");
// ns.BaseNamespace == "MyApp.Data"
// ns.DaoNamespace == "MyApp.Data.Dao"
// ns.WrapperNamespace == "MyApp.Data.Wrappers"
```

## Known Gaps / Not Yet Implemented

The test suite is minimal, covering only basic column, table, and namespace functionality. Schema extraction, DAO generation, and foreign key/xref handling are not tested in this project.
