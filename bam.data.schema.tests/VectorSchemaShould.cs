using Bam.Data.Schema;
using Bam.DependencyInjection;
using Bam.Test;

namespace Bam.Data.Schema.Tests
{
    [UnitTestMenu("Vector schema should", Selector = "vss")]
    public class VectorSchemaShould : UnitTestMenuContainer
    {
        public VectorSchemaShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        [UnitTest]
        public void MapVectorColumnAcrossTypeSurfaces()
        {
            Table table = new Table("Episode");
            table.AddColumn("Embedding", DataTypes.Vector);
            Column column = (Column)table["Embedding"];
            column.MaxLength = "1536";

            When.A<Column>("maps a vector column across the schema type surfaces",
                column,
                (c) => c)
            .TheTest
            .ShouldPass(because =>
            {
                because.ItsTrue("NativeType is the nullable Vector value object", column.NativeType == "Vector?");
                because.ItsTrue("DbDataType is vector", column.DbDataType == "vector");
                because.ItsTrue("MaxLength carries the authored dimension", column.MaxLength == "1536");
            })
            .SoBeHappy()
            .UnlessItFailed();
        }

        [UnitTest]
        public void SatisfyTheCodegenPropertyTemplateContract()
        {
            Table table = new Table("Episode");
            table.AddColumn("Embedding", DataTypes.Vector);
            Column column = (Column)table["Embedding"];
            column.MaxLength = "1536";

            When.A<Column>("resolves the tokens Property.tmpl renders for a vector column",
                column,
                (c) =>
                {
                    // the same token substitutions Property.tmpl performs:
                    //   Get@(Model.DataType)Value -> the Dao accessor name
                    //   DbDataType/MaxLength      -> the generated ColumnAttribute arguments
                    string accessorName = $"Get{c.DataType}Value";
                    string attributeFragment = $"[Bam.Data.Column(Name=\"{c.Name}\", DbDataType=\"{c.DbDataType}\", MaxLength=\"{c.MaxLength}\", AllowNull={c.AllowNull.ToString().ToLower()})]";
                    return new string[] { accessorName, attributeFragment };
                })
            .TheTest
            .ShouldPass(because =>
            {
                string[] results = (string[])because.Result;
                because.ItsTrue("the generated getter resolves to the Dao vector accessor", results[0] == "GetVectorValue");
                because.ItsTrue("the generated attribute declares the vector db type and dimension", results[1].Contains("DbDataType=\"vector\", MaxLength=\"1536\""));
            })
            .SoBeHappy()
            .UnlessItFailed();
        }

        [UnitTest]
        public void LeaveVectorDimensionToTheSchemaAuthor()
        {
            Table table = new Table("Episode");
            table.AddColumn("Embedding", DataTypes.Vector);
            Column column = (Column)table["Embedding"];

            When.A<Column>("does not invent a default vector dimension",
                column,
                (c) => c.DbDataType)
            .TheTest
            .ShouldPass(because =>
            {
                because.ItsTrue("DbDataType is vector", (string)because.Result == "vector");
                because.ItsTrue("MaxLength stays unset until the author supplies the dimension", string.IsNullOrEmpty(column.MaxLength));
            })
            .SoBeHappy()
            .UnlessItFailed();
        }
    }
}
