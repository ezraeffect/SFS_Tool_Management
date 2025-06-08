using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using SFS_Tool_Management.Repositories;
using SFS_Tool_Management.Models;
using System.Windows;
using System.Windows.Input;
using SFS_Tool_Management.Data;

namespace SFS_Tool_Management.ViewModels
{
    public partial class AdminViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserList> users;
        [ObservableProperty]
        private UserList selectedUser;
        [ObservableProperty]
        private bool isPopupOpen;

        private readonly SQLRepository sqlRepository;
        public AdminViewModel()
        {
            sqlRepository = new SQLRepository();
            RefreshUsersAsync();
        }
        private async Task RefreshUsersAsync()
        {
            string query = "SELECT UserID, Name, Position, Department, PhoneNumber, IsAdmin FROM [dbo].[UserList]";
            List<UserList> list = await sqlRepository.ExecuteQueryAsync(query, reader =>
            {
                return new UserList
                {
                    UserID = reader["UserID"] as string,
                    Name = reader["Name"] as string,
                    Position = reader["Position"] as string,
                    Department = reader["Department"] as string,
                    PhoneNumber = reader["PhoneNumber"] as string,
                    IsAdmin = (reader["IsAdmin"] is bool b && b)
                };
            });
            Users = new ObservableCollection<UserList>(list);
        }
        private bool CanModifyUser(UserList? user) => user != null;

        [RelayCommand(CanExecute = nameof(CanModifyUser))]
        private async Task DeleteUser(UserList? user)
        {
            if (user == null)
                return;

            string query = "DELETE FROM [dbo].[UserList] WHERE UserID = @UserID";
            using (var conn = new SqlConnection(SQLRepository.BuildConnectionString()))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", user.UserID);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            await RefreshUsersAsync();
        }

        [RelayCommand(CanExecute = nameof(CanModifyUser))]
        private void OpenPopup(UserList? user)
        {
            if (user == null)
                return;
            SelectedUser = user;
            IsPopupOpen = true;
        }

        [RelayCommand]
        private async Task SaveEdit()
        {
            if (SelectedUser == null)
                return;

            string query = "UPDATE [dbo].[UserList] SET Position = @Position, Department = @Department WHERE UserID = @UserID";
            using (var conn = new SqlConnection(SQLRepository.BuildConnectionString()))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Position", SelectedUser.Position);
                cmd.Parameters.AddWithValue("@Department", SelectedUser.Department);
                cmd.Parameters.AddWithValue("@UserID", SelectedUser.UserID);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            await RefreshUsersAsync();
            IsPopupOpen = false;
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsPopupOpen = false;
        }
    }
}
