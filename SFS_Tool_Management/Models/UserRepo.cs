using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using SFS_Tool_Management.ViewModels;


namespace SFS_Tool_Management.Models
{
    public class UserRepo : UserViewModel
    {
        public static List<UserList> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.UserList.ToList();
            }
        }
        private static UserList? currentUser;
        public UserList CurrentUser
        {
            get => currentUser ?? new UserList();
            set
            {
                currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(Department));
            }
        }
        public string? UserName => CurrentUser?.Name;
        public string? Department => CurrentUser?.Department;


        public static void AddUser(UserList user)
        {
            using (var db = new AppDbContext())
            {
                db.UserList.Add(user);
                db.SaveChanges();
            }
        }
        public static void SetCurrentUser(UserList user)
        {
            currentUser = user;
        }
        public static UserList? GetCurrentUser()
        {
            return currentUser;
        }

    }
}
