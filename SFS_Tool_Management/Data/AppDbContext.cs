using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFS_Tool_Management.Models;
using System.Data.SqlClient;

namespace SFS_Tool_Management.Data
{
    public class AppDbContext : DbContext
    {
        string connectionString = string.Format("sfstool","1433", "SFS", "Codingon", "sfs2751!", false);
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=AppDb;Integrated Security=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
