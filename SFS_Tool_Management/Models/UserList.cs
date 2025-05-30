using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace SFS_Tool_Management.Models
{
    public class UserList
    {
        [Key]
        public string? UserID { get; set; }
        public string? Name { get; set; }
        public string? PasswordHash { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? PhoneNumber {  get; set; }
        public bool IsAdmin { get; set; }
        public UserList() { }

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
