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
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace SFS_Tool_Management.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserList>? userLists;
        [ObservableProperty]
        private UserList? currentUser;

        public IRelayCommand? ShowAdminOptionCommand { get; }

        public UserViewModel()
        {
            var users = UserList.GetAllUsers() ?? new List<UserList>();
            UserLists = new ObservableCollection<UserList>(users);

            CurrentUser = UserList.Instance.CurrentUser;
            UserList.Instance.PropertyChanged += User_PropertyChanged;
        }
        private void User_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserList.CurrentUser))
            {
                CurrentUser = UserList.Instance.CurrentUser;
            }
        }
    }
}
