using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SFS_Tool_Management.Models;

namespace SFS_Tool_Management.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly UserItem user;
    }
}
