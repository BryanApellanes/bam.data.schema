namespace Bam.Data.Schema
{
    public enum DaoSchemaExtractorNamingCollisionStrategy
    {
        Invalid,
        TypeSuffix,
        TypePrefix,
        TrailingUnderscore,
        LeadingUnderscore,
        UnderscoreDelimit,
        Custom
    }
}
