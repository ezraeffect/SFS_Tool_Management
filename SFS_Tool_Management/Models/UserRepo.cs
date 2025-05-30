using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace SFS_Tool_Management.Models
{
    public class UserRepo
    {
        public static List<UserList> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.UserList.ToList();
            }
        }
        public static void AddUser(UserList user)
        {
            using (var db = new AppDbContext())
            {
                db.UserList.Add(user);
                db.SaveChanges();
                MessageBox.Show($"입력한 비밀번호: {user.PasswordHash}");
            }
            MessageBox.Show($"입력한 비밀번호: {user.PasswordHash}");
        }
    }
}
