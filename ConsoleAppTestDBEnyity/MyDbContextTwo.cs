using ConsoleAppTestDBEnyity.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTestDBEnyity
{
    class MyDbContextTwo : DbContext
    {
        public DbSet<Test> Tests { get; set; }

        public MyDbContextTwo(/*DbContextOptions<MyDbContext> options*/) /*: base(options)*/
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source=test2.db");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Test>().Property(test => test.TestValue);
        }
    }
}
