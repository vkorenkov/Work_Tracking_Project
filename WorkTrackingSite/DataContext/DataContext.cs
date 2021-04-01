﻿using Install_Printers_Lib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTrackingSite.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Printer> Printers { get; set; }

        public DbSet<Link> Links { get; set; }

        public DataContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optBuilder)
        {
            //optBuilder.UseSqlServer(
            //    @"Server=SKY-244\OKS_SQL;
            //    DataBase=Install_Printers;
            //    Integrated Security=True");

            //optBuilder.UseSqlServer(
            //    @"Server=M1-NUT-TNI\DB_OKS;
            //    DataBase=Install_Printers;
            //    Integrated Security=True");
        }
    }
}
