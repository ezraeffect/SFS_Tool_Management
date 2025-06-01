using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.CompilerServices;
using SFS_Tool_Management.Views;
using System.Windows;

namespace SFS_Tool_Management.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object? _currentPage;
        private bool isLoggedIn;
        public static MainViewModel Instance { get; } = new MainViewModel();
        public MainViewModel()
        {
            OpenLoginCommand = new RelayCommand<object>(param => OpenLogin());
        }
        public bool IsLoggedIn
        {
            get => isLoggedIn;
            set
            {
                isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsBtnVisible));
            }
        }
        public Visibility IsBtnVisible => IsLoggedIn ? Visibility.Collapsed : Visibility.Visible;
        public object? CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        public ICommand OpenLoginCommand { get; }
        private void OpenLogin()
        {
            SignInWindow signIn = new SignInWindow();
            signIn.Show();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
