namespace Bam.Data.Schema
{
    /// <summary>
    /// Defines strategies for resolving naming collisions between database column names and reserved DAO keywords during schema extraction.
    /// </summary>
    public enum DaoSchemaExtractorNamingCollisionStrategy
    {
        /// <summary>
        /// Invalid strategy; throws an exception if used.
        /// </summary>
        Invalid,
        /// <summary>
        /// Appends the data type name as a suffix to the property name.
        /// </summary>
        TypeSuffix,
        /// <summary>
        /// Prepends the data type name as a prefix to the property name.
        /// </summary>
        TypePrefix,
        /// <summary>
        /// Appends an underscore to the end of the property name.
        /// </summary>
        TrailingUnderscore,
        /// <summary>
        /// Prepends an underscore to the beginning of the property name.
        /// </summary>
        LeadingUnderscore,
        /// <summary>
        /// Wraps the property name with underscores on both sides.
        /// </summary>
        UnderscoreDelimit,
        /// <summary>
        /// Uses a custom handler function for resolving collisions.
        /// </summary>
        Custom
    }
}
