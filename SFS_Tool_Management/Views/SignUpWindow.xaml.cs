using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
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
            PasswordBox? pb = sender as PasswordBox;
            if (pb != null)
            {
                var viewModel = this.DataContext as SignUpViewModel; // ViewModel 가져오기
                if (viewModel != null)
                {
                    viewModel.Password = pb.Password; // ViewModel의 Password 업데이트
                }
            }
        }
    }
}
