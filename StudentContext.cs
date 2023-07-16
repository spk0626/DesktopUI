using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DesktopUI
{
    public class StudentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students.db");

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Ignore(s => s.Image);

            modelBuilder.Entity<Student>()
                .Property(s => s.ImageBytes)
                .HasColumnType("BLOB"); 
        }
        
    }
}
