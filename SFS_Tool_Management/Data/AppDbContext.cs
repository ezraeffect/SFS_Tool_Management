using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFS_Tool_Management.Models;
using SFS_Tool_Management.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Configuration;

namespace SFS_Tool_Management.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserList> UserList { get; set; }
        private string connectionString = "Server=***REMOVED***;Database=Tool;User Id=***REMOVED***; Password=***REMOVED***;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(SQLRepository.BuildConnectionString());
            }
        }
        public List<UserList> GetAllUsers()
        {
            return UserList.ToList();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserList>()
                   .Ignore(u => u.CurrentUser);
        }

    }
}
