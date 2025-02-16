namespace Bam.Data.Schema
{
    public class CuidSchemaManager: UuidSchemaManager
    {
        public CuidSchemaManager(bool autoSave = true) : base(autoSave)
        {
            PreColumnAugmentations.Add(new AddColumnAugmentation { ColumnName = "Cuid", DataType = DataTypes.String, AllowNull = true });
        }
    }
}
