using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SFS_Tool_Management.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private UserRepo _repo;
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
            _repo = new UserRepo();
            UserLists = new ObservableCollection<UserList> (UserRepo.GetAllUsers());
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
