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

namespace SFS_Tool_Management
{
    /// <summary>
    /// SignUpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }
        private void button_SignUp_Click(object sender, RoutedEventArgs e)
        {
            List<User> users = User.users;
            string name = textBox_Name.Text;
            string pos = textBox_Position.Text;
            string dep = textBox_Department.Text;
            string PN = textBox_PhoneNumber.Text;
            string id = textBox_ID.Text;
            string pw = textBox_Password.Password;
            bool ac = false;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("이름을 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(pos))
            {
                MessageBox.Show("직급을 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(dep))
            {
                MessageBox.Show("부서를 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(PN))
            {
                MessageBox.Show("연락처를 입력하세요.");
                return;
            }
            if (!(Regex.IsMatch(PN, @"^010\d{8}$") || Regex.IsMatch(PN, @"^0[2-6]\d{7,8}$")))
            {
                MessageBox.Show("올바른 전화번호 형식이 아닙니다.");
                return;
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("아이디를 입력하세요.");
                return;
            }
            if (id.Length < 8 || id.Length > 16)
            {
                MessageBox.Show("아이디는 8자 이상 16자 이하여야 합니다.");
                return;
            }
            if (string.IsNullOrWhiteSpace(pw))
            {
                MessageBox.Show("비밀번호를 입력하세요.");
                return;
            }
            if (pw.Length < 8 || pw.Length > 20)
            {
                MessageBox.Show("비밀번호는 8자 이상 20자 이하여야 합니다.");
                return;
            }
            if (!Regex.IsMatch(pw, @"[^가-힣ㄱ-ㅎㅏ-ㅣ]"))
            {
                MessageBox.Show("비밀번호는 대문자, 소문자, 특수문자로만 구성되어야 합니다.");
                return;
            }
            if (!Regex.IsMatch(pw, @"\d"))
            {
                MessageBox.Show("비밀번호에는 최소 하나 이상의 숫자가 포함되어야 합니다.");
                return;
            }
            if (!Regex.IsMatch(pw, @"[~․!@#$%^&*()_\-+={}[\]|\:;""<>,.?/]$"))
            {
                MessageBox.Show("비밀번호에는 최소 하나 이상의 특수문자가 포함되어야 합니다.");
                return;
            }
        }
    }
}
