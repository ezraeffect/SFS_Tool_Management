using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Metadata;
using SFS_Tool_Management.Models;
using SFS_Tool_Management.Repositories;

namespace SFS_Tool_Management.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Frame _mainFrame;
        private readonly string _userID;

        public MainWindowViewModel(Frame mainFrame, string userID)
        {
            _mainFrame = mainFrame;
            _userID = userID;
        }

        [ObservableProperty]
        public string currentPageName;

        [ObservableProperty]
        public UserModel currentUser;

        [RelayCommand]
        public async Task GetCurrentUserInfo()
        {
            var repo = new SQLRepository();

            string query = @"SELECT UserID, Name, Position, Department, IsAdmin, ImageBinary 
                     FROM dbo.UserList 
                     WHERE UserID = @UserID";

            var parameters = new Dictionary<string, object>
            {
                { "@UserID", _userID }
            };

            var result = await repo.ExecuteQueryAsync(query, reader => new UserModel
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                Position = reader.GetString(2),
                Department = reader.GetString(3),
                IsAdmin = reader.GetBoolean(4),
                Image = reader["ImageBinary"] as byte[]
            }, parameters);

            if (result.Count > 0)
                CurrentUser = result[0];
        }


        [RelayCommand]
        public void ShowDashboard()
        {
            CurrentPageName = "Dashboard";
            _mainFrame.Navigate(new Views.DashboardPage(_userID));
        }

        [RelayCommand]
        public void ShowToolList()
        {
            CurrentPageName = "Tool List";
            _mainFrame.Navigate(new Views.ToolListPage(_userID));
        }

        [RelayCommand]
        public void ShowRentalHistory()
        {
            CurrentPageName = "Rental History";
            _mainFrame.Navigate(new Views.RentalHistoryPage(_userID));
        }

        [RelayCommand]
        public void ShowRepairHistory()
        {
            CurrentPageName = "Repair History";
            _mainFrame.Navigate(new Views.Repair.RepairHistoryPage(_userID));
        }

        [RelayCommand]
        public void ShowAdminPage()
        {
            CurrentPageName = "회원 관리 (관리자 페이지)";
            _mainFrame.Navigate(new Views.AdminOptionPage());
        }
    }
}
