using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Data.SqlClient;
using SFS_Tool_Management.Models;
using SFS_Tool_Management.ViewModels;
using SFS_Tool_Management.Data;

namespace SFS_Tool_Management.Views
{
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
            DataContext = new SignInViewModel();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb && DataContext is SignInViewModel viewModel)
            {
                viewModel.Password = pb.Password;
            }
        }
        private void SignUpTextBlock_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as SignInViewModel;
            viewModel?.OpenSignUpPage();
        }

    }
}
