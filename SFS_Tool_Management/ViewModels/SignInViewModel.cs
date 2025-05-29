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
                    var user = db.UserLists.FirstOrDefault(u => u.ID == ID);
                    if (string.IsNullOrWhiteSpace(ID))
                    {
                        MessageBox.Show("아이디를 입력하세요.");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        MessageBox.Show("비밀번호를 입력하세요.");
                        return;
                    }
                    if (user == null)
                    {
                        MessageBox.Show("존재하지 않는 아이디입니다.");
                        return;
                    }
                    if (user.Hashedpw != Encrypter.HashPW(Password))
                    {
                        MessageBox.Show("비밀번호가 틀렸습니다.");
                        return;
                    }

                    MessageBox.Show("로그인 완료");
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Application.Current.MainWindow.Close();
                }
            }
            catch ()
            {

            }
        }
        private void OpenSignUpPage()
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
        }
    }
}
