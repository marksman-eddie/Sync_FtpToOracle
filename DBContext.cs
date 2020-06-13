using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;

namespace Sync_FtpToOracle
{
    public class OrdersDBContext : DbContext
    {
        public OrdersDBContext(DbContextOptions <OrdersDBContext> options): base(options)
        {
           
        }
        public DbSet<Orderdoc> ORDERDOC { get; set; }

    }
}
