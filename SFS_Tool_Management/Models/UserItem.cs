using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SFS_Tool_Management.Models
{
    public class UserItem
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserDepartmet { get; set; }
        public string UserPhoneNumber {  get; set; }
        public string UserIsAdmin { get; set; }
    }

    public class SQLcInfo
    {
        public string Server {  get; set; }
        public string Port {  get; set; }
        public string Uid { get; set; }
        public string PWD { get; set; }
        public string Database {  get; set; }

        public string GetConnectInfo()
        {
            return string.Format($"Data Source={Server},{Port};" +
                                 $"Initial Catalog={Database};" +
                                 $"User ID={Uid};" +
                                 $"Password={PWD};" +
                                 $"Integrated Security={false};" +
                                 $"Connection Timeout=1");
        }
    }
    
}
