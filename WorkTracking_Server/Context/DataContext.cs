using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTrackingLib;
using WorkTrackingLib.Models;

namespace WorkTracking_Server.Context
{
    public class DataContext : DbContext
    {
        public DbSet<NewWrite> ComplitedWorks { get; set; }

        public DbSet<RepairClass> Repairs { get; set; }

        public DbSet<Devices> Devices { get; set; }

        public DbSet<Admins> Admins { get; set; }

        public DbSet<Osp> Osp { get; set; }

        public DbSet<OsType> OsType { get; set; }

        public DbSet<Results> Results { get; set; }

        public DbSet<Why> Why { get; set; }

        public DbSet<ScOks> ScOks { get; set; }

        public DbSet<RepairsStatus> RepairsStatuses { get; set; }

        public DataContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optBuilder)
        {
            //optBuilder.UseSqlServer(
            //    @"Server=SKY-244\OKS_SQL;
            //    DataBase=PassYourWork;
            //    Integrated Security=True");

            optBuilder.UseSqlServer(
                @"Server=SKY-244\OKS_SQL;
                DataBase=TestWorkTracking;
                Integrated Security=True");

            //optBuilder.UseSqlServer(
            //    @"Server=(localdb)\MSSQLLocalDB;
            //    DataBase=PassYourWork;
            //    Integrated Security=True");
        }
    }
}
