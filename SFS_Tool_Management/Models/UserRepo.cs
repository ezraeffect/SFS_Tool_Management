using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Data;
using System.Data.SqlClient;

namespace SFS_Tool_Management.Models
{
    public class UserRepo
    {
        public static List<UserList> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.UserLists.ToList();
            }
        }
        public static void AddUser(UserList user)
        {
            using (var db = new AppDbContext())
            {
                db.UserLists.Add(user);
                db.SaveChanges();
            }
        }
    }
}
