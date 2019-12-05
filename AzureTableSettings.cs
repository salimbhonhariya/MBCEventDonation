using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBCEventDonation
{
    public class AzureTableSettings
    {
        public AzureTableSettings(string connectionString, string tableName)
        {
            TableName = tableName;
            ConnectionString = connectionString;
        }
        public string TableName
        {
            get;
        }
        public string ConnectionString
        {
            get;
            set;
        }
    }
}
