using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace AusGrid.Models
{
    public class GetConnection
    {
        public string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            return connectionString;
        }

    }
}