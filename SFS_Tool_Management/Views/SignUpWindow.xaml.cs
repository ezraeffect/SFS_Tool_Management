using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using SFS_Tool_Management.Models;
using SFS_Tool_Management.ViewModels;

namespace SFS_Tool_Management.Views
{
    /// <summary>
    /// SignUpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
            DataContext = new SignUpViewModel();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb && DataContext is SignUpViewModel viewModel)
            {
                viewModel.Password = pb.Password;
            }
        }
    }
}
