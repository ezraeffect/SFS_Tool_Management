using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Data;

namespace SFS_Tool_Management.Models
{
    public class UserRepo
    {
        private static List<User> users = new List<User>
        {
            new User ("a", "1234", "supervisor", "engineer", "01012345678", false)
        };
        public static List<User> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.Users.ToList();
            }
        }
        public static void AddUser(User user)
        {
            using (var db = new AppDbContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

    }
}
