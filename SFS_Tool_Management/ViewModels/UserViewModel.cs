using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SFS_Tool_Management.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<UserList>? _userLists;
        public ObservableCollection<UserList>? UserLists
        {
            get => _userLists;
            set
            {
                _userLists = value;
                OnPropertyChanged(nameof(UserLists));
            } 
        }
        public UserViewModel()
        {
            var users = UserList.GetAllUsers() ?? new List<UserList>();
            UserLists = new ObservableCollection<UserList>(users);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
