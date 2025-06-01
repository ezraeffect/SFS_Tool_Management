using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using SFS_Tool_Management.Models;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SFS_Tool_Management.Views;
using SFS_Tool_Management.Data;
using SFS_Tool_Management.ViewModels;

namespace SFS_Tool_Management.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string? ID { get; set; }
        public string? Password { get; set; }
        public ICommand SignInCommand { get; }
        public ICommand SignUpCommand { get; }

        public SignInViewModel()
        {
            SignInCommand = new RelayCommand(_ => SignIn());
            SignUpCommand = new RelayCommand(_ => OpenSignUpPage());
        }

        private void SignIn()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var user = db.UserList.FirstOrDefault(u => u.UserID == ID);
                    if (string.IsNullOrWhiteSpace(ID))
                    {
                        MessageBox.Show("아이디를 입력하세요.", "로그인 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        MessageBox.Show("비밀번호를 입력하세요.", "로그인 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (user == null)
                    {
                        MessageBox.Show("존재하지 않는 아이디입니다.", "로그인 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string hashedPassword = Encrypter.HashPW(Password);
                    if (user.PasswordHash != hashedPassword)
                    {
                        MessageBox.Show("비밀번호가 틀렸습니다.", "로그인 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    MessageBox.Show($"{user.Name}님, 환영합니다.", "로그인 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Application.Current.MainWindow.Close();
                    UserRepo.SetCurrentUser(user);
                    MainViewModel.Instance.IsLoggedIn = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"로그인 중 오류가 발생했습니다: {ex.Message}");
            }
        }
        private void OpenSignUpPage()
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
        }
    }
}
