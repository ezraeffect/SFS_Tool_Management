using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SFS_Tool_Management.ViewModels
{
    public partial class SignUpViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;
        [ObservableProperty]
        private string position = string.Empty;
        [ObservableProperty]
        private string department = string.Empty;
        [ObservableProperty]
        private string phoneNumber = string.Empty;
        [ObservableProperty]
        private string iD = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isAdmin = false;

        [RelayCommand]
        private void SignUp()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("이름을 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(Position))
            {
                MessageBox.Show("직급을 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(Department))
            {
                MessageBox.Show("부서를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("연락처를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!(Regex.IsMatch(PhoneNumber, @"^010\d{8}$") || Regex.IsMatch(PhoneNumber, @"^0[2-6]\d{7,8}$")))
            {
                MessageBox.Show("올바른 전화번호 형식이 아닙니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(ID))
            {
                MessageBox.Show("아이디를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ID.Length < 8 || ID.Length > 16)
            {
                MessageBox.Show("아이디는 8자 이상 16자 이하여야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Password.Length < 8 || Password.Length > 20)
            {
                MessageBox.Show("비밀번호는 8자 이상 20자 이하여야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Regex.IsMatch(Password, @"[^가-힣ㄱ-ㅎㅏ-ㅣ]"))
            {
                MessageBox.Show("비밀번호는 대문자, 소문자, 특수문자로만 구성되어야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Regex.IsMatch(Password, @"\d"))
            {
                MessageBox.Show("비밀번호에는 최소 하나 이상의 숫자가 포함되어야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Regex.IsMatch(Password, @"[~․!@#$%^&*()_\-+={}[\]|\:;""<>,.?/]"))
            {
                MessageBox.Show("비밀번호에는 최소 하나 이상의 특수문자가 포함되어야 합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool ac = (IsAdmin) ? true : false;
            string hashedPw = Encrypter.HashPW(Password);
            UserList newUser = new UserList(Name, ID, Position, Department, PhoneNumber, ac, hashedPw);
            UserList.AddUser(newUser);

            MessageBox.Show("회원가입이 완료되었습니다.", "회원가입 완료", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                Password = pb.Password;
            }
        }
    }
}
