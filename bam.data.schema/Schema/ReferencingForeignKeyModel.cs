namespace Bam.Data.Schema
{
    public class ReferencingForeignKeyModel
    {
        public ReferencingForeignKeyModel(IForeignKeyColumn foreignKey)
        {
            Model = foreignKey;
        }

        public IForeignKeyColumn Model { get; set; }
        
        public string PropertyName
        {
            get
            {
                return $"{Model.ReferencingClass.Pluralize()}By{Model.Name}";
            }
        }
    }
}
