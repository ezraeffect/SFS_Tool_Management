using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace SFS_Tool_Management.Models
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
            Hashedpw = Encrypter.HashPW(pw);
            Position = pos;
            Department = dep;
            PN = pn;
            Access = ac;
        }
    }
}
