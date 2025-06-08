using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SFS_Tool_Management.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SFS_Tool_Management.Models
{
    public partial class UserList : ObservableObject
    {
        [property: Key]
        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? userID;

        [ObservableProperty]
        private string? position;

        [ObservableProperty]
        private string? department;

        [ObservableProperty]
        public string? passwordHash;

        [ObservableProperty]
        public string? phoneNumber;

        public bool IsAdmin { get; set; }

        public byte[]? ImageBinary { get; set; }
        public string Access => IsAdmin ? "Administrator" : "User";

        public UserList() { }

        private static UserList? instance;
        public static UserList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserList();
                }
                return instance;
            }
        }
        [NotMapped]
        [ObservableProperty]
        private UserList? currentUser;
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
        public void SetCurrentUser(UserList user)
        {
            CurrentUser = user;
            OnPropertyChanged(nameof(CurrentUser));
        }
        public UserList(string? name, string? id, string? pos, string? dep, string? pn, bool ac, string? pw, byte[]? ib)
        {
            Name = name;
            UserID = id;
            PasswordHash = pw;
            Position = pos;
            Department = dep;
            PhoneNumber = pn;
            IsAdmin = ac;
            ImageBinary = ib;
        }
        public string DisplayName
            => $"{Position} {Name}";
    }
}
