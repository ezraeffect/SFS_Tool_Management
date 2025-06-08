using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SFS_Tool_Management.Models
{
    public partial class UserModel : ObservableObject
    {
        [ObservableProperty] public string? id;
        [ObservableProperty] public string? name;
        [ObservableProperty] public string? position;
        [ObservableProperty] public string? department;
        [ObservableProperty] public byte[]? image;
        [ObservableProperty] public bool? isAdmin;
    }
}
