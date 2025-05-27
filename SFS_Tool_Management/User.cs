using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SFS_Tool_Management
{
    public class User
    {
        public string ID { get; set; }
        public string Hashedpw { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string PN {  get; set; }
        public bool Access { get; set; }
        public User(string id, string pw, string pos, string dep, string pn, bool ac)
        {
            ID = id;
            Hashedpw = HashPW(pw);
            Position = pos;
            Department = dep;
            PN = pn;
            Access = ac;
        }
        public static string HashPW(string pw)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pw));
                return Convert.ToBase64String(bytes);
            }
        }
        public static List<User> users = new List<User>
        {
            new User ("a", "1234", "supervisor", "engineer", "01012345678", false)
        };
    }
}
