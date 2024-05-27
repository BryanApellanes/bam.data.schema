using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
