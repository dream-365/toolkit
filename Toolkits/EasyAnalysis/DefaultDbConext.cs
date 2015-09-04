using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyAnalysis
{
    public class DefaultDbConext : DbContext
    {
        public DefaultDbConext() : base ("DefaultConnection") 
        {
        }

        public DbSet<ThreadModel> Threads { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ThreadTypeConfigration());

            base.OnModelCreating(modelBuilder);
        }

    }
}