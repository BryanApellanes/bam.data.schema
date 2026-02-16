namespace Bam.Data.Schema
{
    /// <summary>
    /// A schema manager that adds Id, Uuid, and Cuid columns to every table. Extends <see cref="UuidSchemaManager"/> by adding a string "Cuid" column.
    /// </summary>
    public class CuidSchemaManager: UuidSchemaManager
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CuidSchemaManager"/>, adding a Cuid column augmentation.
        /// </summary>
        /// <param name="autoSave">Whether to automatically save schema changes to disk.</param>
        public CuidSchemaManager(bool autoSave = true) : base(autoSave)
        {
            PreColumnAugmentations.Add(new AddColumnAugmentation { ColumnName = "Cuid", DataType = DataTypes.String, AllowNull = true });
        }
    }
}
