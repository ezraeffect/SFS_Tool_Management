using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFS_Tool_Management.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Configuration;

namespace SFS_Tool_Management.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserList> UserList { get; set; }
        private string connectionString = "Server=sfstool.database.windows.net;Database=Tool;User Id=Codingon; Password=sfs2751!;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public List<UserList> GetAllUsers()
        {
            return UserList.ToList();
        }
    }
}
