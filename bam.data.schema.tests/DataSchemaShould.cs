using Bam.Data.Schema;
using Bam.DependencyInjection;
using Bam.Test;

namespace Bam.Data.Schema.Tests
{
    [UnitTestMenu("DataSchema Should", Selector = "dss")]
    public class DataSchemaShould : UnitTestMenuContainer
    {
        public DataSchemaShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        [UnitTest]
        public void CreateColumnWithExpectedDefaults()
        {
            Column column = new Column("Id", "TestTable");

            When.A<Column>("is created with name and table",
                column,
                (c) => c)
            .TheTest
            .ShouldPass(because =>
            {
                because.ItsTrue("Name is Id", column.Name == "Id");
                because.ItsTrue("TableName is TestTable", column.TableName == "TestTable");
                because.ItsTrue("DataType is ULong", column.DataType == DataTypes.ULong);
                because.ItsTrue("AllowNull is false", column.AllowNull == false);
                because.ItsTrue("Key is true", column.Key == true);
            })
            .SoBeHappy()
            .UnlessItFailed();
        }

        [UnitTest]
        public void CreateTableWithColumns()
        {
            Table table = new Table("TestTable");
            table.AddColumn("Name", DataTypes.String);
            table.AddColumn("Age", DataTypes.Int);

            When.A<Table>("has columns added",
                table,
                (t) => t)
            .TheTest
            .ShouldPass(because =>
            {
                because.ItsTrue("Name is TestTable", table.Name == "TestTable");
                because.ItsTrue("has 2 columns", table.Columns.Length == 2);
                because.ItsTrue("has Name column", table.HasColumn("Name"));
                because.ItsTrue("has Age column", table.HasColumn("Age"));
            })
            .SoBeHappy()
            .UnlessItFailed();
        }

        [UnitTest]
        public void SetDataNamespacesFromBaseNamespace()
        {
            DataNamespaces ns = new DataNamespaces("MyApp.Data");

            When.A<DataNamespaces>("is created with a base namespace",
                ns,
                (n) => n)
            .TheTest
            .ShouldPass(because =>
            {
                because.ItsTrue("BaseNamespace is MyApp.Data", ns.BaseNamespace == "MyApp.Data");
                because.ItsTrue("DaoNamespace is MyApp.Data.Dao", ns.DaoNamespace == "MyApp.Data.Dao");
                because.ItsTrue("WrapperNamespace is MyApp.Data.Wrappers", ns.WrapperNamespace == "MyApp.Data.Wrappers");
            })
            .SoBeHappy()
            .UnlessItFailed();
        }
    }
}
