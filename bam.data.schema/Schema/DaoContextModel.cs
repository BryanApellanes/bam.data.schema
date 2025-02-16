namespace Bam.Data.Schema
{
    public class DaoContextModel
    {
        public IDaoSchemaDefinition Model { get; set; }
        public string Namespace { get; set; }

        public DaoTableSchemaModel[] Tables
        {
            get
            {
                return Model.Tables.Select(t => new DaoTableSchemaModel { Model = t, Namespace = Namespace, Schema = Model }).ToArray();
            }
        }
    }
}
