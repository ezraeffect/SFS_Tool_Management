using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SFS_Tool_Management.Data;
using System.Reflection.Metadata.Ecma335;

namespace SFS_Tool_Management.Models
{
    public class UserList : INotifyPropertyChanged
    {
        private string? name, id, position, department;
        [Key]
        public string? Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string? UserID
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(UserID));
                }
            }
        }


        public string? Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }
        public string? Department
        {
            get => department;
            set
            {
                if (department != value)
                {
                    department = value;
                    OnPropertyChanged(nameof(Department));
                }
            }
        }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        public UserList() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        public static event PropertyChangedEventHandler? PropertyChangedStatic;
        private static UserList? currentUser;
        public static UserList? CurrentUser
        {
            get => currentUser;
            set
            {
                if (currentUser != value)
                {
                    currentUser = value;
                    PropertyChangedStatic?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentUser)));
                }
            }
        }
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
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public UserList(string name, string id, string pos, string dep, string pn, bool ac, string pw)
        {
            Name = name;
            UserID = id;
            PasswordHash = pw;
            Position = pos;
            Department = dep;
            PhoneNumber = pn;
            IsAdmin = ac;
        }
    }
}
