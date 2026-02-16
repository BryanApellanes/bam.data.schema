using Bam.Data.Npgsql;

namespace Bam.Data.Postgres
{
    /// <summary>
    /// Extracts schema definitions from a PostgreSQL database. An alias for <see cref="NpgsqlSchemaExtractor"/>.
    /// </summary>
    public class PostgresSchemaExtractor: NpgsqlSchemaExtractor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PostgresSchemaExtractor"/> for the specified database.
        /// </summary>
        /// <param name="database">The Npgsql database to extract schema from.</param>
        public PostgresSchemaExtractor(NpgsqlDatabase database) : base(database)
        {
        }
    }
}
