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
        public DbSet<UserList> UserLists { get; set; }
        private SqlConnection? conn;
        private string connectionString = "Server=***REMOVED***;Database=SFS;" +
            "User Id=***REMOVED***; Password=***REMOVED***;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=***REMOVED***;Database=SFS;" +
            "User Id=***REMOVED***; Password=***REMOVED***;";
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public List<UserList> GetAllUsers()
        {
            List<UserList> users = new List<UserList>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM UserList";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserList
                        {
                            Name = reader["Name"].ToString(),
                            ID = reader["UserID"].ToString(),
                            Hashedpw = reader["PasswordHash"].ToString(),
                            Position = reader["Position"].ToString(),
                            Department = reader["Department"].ToString(),
                            PN = reader["PhoneNumber"].ToString(),
                            Access = Convert.ToBoolean(reader["IsAdmin"])
                        });
                    }
                }
            }
            return users;
        }

    }
}
