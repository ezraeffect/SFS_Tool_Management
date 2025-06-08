using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SFS_Tool_Management.Models;

namespace SFS_Tool_Management.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Frame _mainFrame;

        public MainWindowViewModel(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }

        [RelayCommand]
        public void ShowDashboard()
        {
            _mainFrame.Navigate(new Views.DashboardPage());
        }

        [RelayCommand]
        public void ShowToolList()
        {
            _mainFrame.Navigate(new Views.ToolListPage());
        }

        [RelayCommand]
        public void ShowRentalHistory()
        {
            _mainFrame.Navigate(new Views.RentalHistoryPage());
        }

        [RelayCommand]
        public void ShowAdminPage()
        {
            _mainFrame.Navigate(new Views.AdminOptionPage());
        }
    }
}
