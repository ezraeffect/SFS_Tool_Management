using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SFS_Tool_Management.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserList>? userLists;
        public UserViewModel()
        {
            var users = UserList.GetAllUsers() ?? new List<UserList>();
            UserLists = new ObservableCollection<UserList>(users);
        }
    }
}
