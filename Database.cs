using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Sync_FtpToOracle
{
    class Database
    {

        public static string ConnectionString =
                          "Data Source=(DESCRIPTION=" +
            "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.12.128.2)(PORT=1521))" +
            ")(CONNECT_DATA=(SERVICE_NAME=AZK)))" +
            ";User Id=gz_tumen;Password=gz_tumen;";

        public static string ConnectionStringTest =
                          "";




    }
}
