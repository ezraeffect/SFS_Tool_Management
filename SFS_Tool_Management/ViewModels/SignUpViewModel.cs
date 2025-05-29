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

namespace SFS_Tool_Management.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        // 회원가입에 필요한 속성들
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _position = string.Empty;
        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(nameof(Position)); }
        }

        private string _department = string.Empty;
        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(nameof(Department)); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(nameof(PhoneNumber)); }
        }

        private string _id = string.Empty;
        public string ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(ID)); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        // 가입 버튼을 위한 Command
        public ICommand SignUpCommand { get; }

        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand(_ => SignUp());
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox? pb = sender as PasswordBox;
            if (pb != null)
            {
                Password = pb.Password;
            }
        }
        public void SignUp()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("이름을 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Position))
            {
                MessageBox.Show("직급을 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Department))
            {
                MessageBox.Show("부서를 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("연락처를 입력하세요.");
                return;
            }
            if (!(Regex.IsMatch(PhoneNumber, @"^010\d{8}$") || Regex.IsMatch(PhoneNumber, @"^0[2-6]\d{7,8}$")))
            {
                MessageBox.Show("올바른 전화번호 형식이 아닙니다.");
                return;
            }
            if (string.IsNullOrWhiteSpace(ID))
            {
                MessageBox.Show("아이디를 입력하세요.");
                return;
            }
            if (ID.Length < 8 || ID.Length > 16)
            {
                MessageBox.Show("아이디는 8자 이상 16자 이하여야 합니다.");
                return;
            }
            if (Password.Length < 8 || Password.Length > 20)
            {
                MessageBox.Show("비밀번호는 8자 이상 20자 이하여야 합니다.");
                return;
            }
            if (!Regex.IsMatch(Password, @"[^가-힣ㄱ-ㅎㅏ-ㅣ]"))
            {
                MessageBox.Show("비밀번호는 대문자, 소문자, 특수문자로만 구성되어야 합니다.");
                return;
            }
            if (!Regex.IsMatch(Password, @"\d"))
            {
                MessageBox.Show("비밀번호에는 최소 하나 이상의 숫자가 포함되어야 합니다.");
                return;
            }
            if (!Regex.IsMatch(Password, @"[~․!@#$%^&*()_\-+={}[\]|\:;""<>,.?/]"))
            {
                MessageBox.Show("비밀번호에는 최소 하나 이상의 특수문자가 포함되어야 합니다.");
                return;
            }
            bool ac = false;
            string hashedPw = Encrypter.HashPW(Password);
            UserList newUser = new UserList(Name, ID, Position, Department, PhoneNumber, ac, hashedPw);
            UserRepo.AddUser(newUser);

            MessageBox.Show("회원가입이 완료되었습니다.");
        }
    }

}
