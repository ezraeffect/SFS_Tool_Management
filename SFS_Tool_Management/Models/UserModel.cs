using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFS_Tool_Management.Models
{
    public partial class UserModel
    {
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? UserPosition { get; set; }
        public string? UserDepartment { get; set; }
        public byte[]? UserImageBlob { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
