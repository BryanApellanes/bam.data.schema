using Bam.Data.Npgsql;

namespace Bam.Data.Postgres
{
    public class PostgresSchemaExtractor: NpgsqlSchemaExtractor
    {
        public PostgresSchemaExtractor(NpgsqlDatabase database) : base(database)
        {
        }
    }
}
