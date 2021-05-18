using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTracking_Server.Context
{
    public class SqlConnectionString
    {
        public string ConnectionString { get; set; }

        public SqlConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
