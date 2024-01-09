/*
	Copyright © Bryan Apellanes 2015  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Data.Schema
{
    /// <summary>
    /// Augments the behavior of a SchemaManager.
    /// </summary>
    public abstract class DaoSchemaManagerAugmentation
    {
        public abstract void Execute(string tableName, DaoSchemaManager manager);
    }
}
