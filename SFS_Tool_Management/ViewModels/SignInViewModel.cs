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
        public string? Message { get; set; }

        public ICommand SignInCommand { get; }
        public ICommand SignUpCommand { get; }

        public SignInViewModel()
        {
            SignInCommand = new RelayCommand(_ => SignIn());
            SignUpCommand = new RelayCommand(_ => OpenSignUpPage());
        }

        private void SignIn()
        {
            if (string.IsNullOrWhiteSpace(ID) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("아이디와 비밀번호를 입력하세요.");
                return;
            }

            var user = UserRepo.GetAllUsers().FirstOrDefault(u => u.ID == ID);
            if (user == null || user.Hashedpw != Encrypter.HashPW(Password))
            {
                MessageBox.Show("아이디 또는 비밀번호가 잘못되었습니다.");
                return;
            }

            MessageBox.Show("로그인 완료");
        }
        private void OpenSignUpPage()
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
        }
    }
}
