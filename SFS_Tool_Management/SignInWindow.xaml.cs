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

namespace SFS_Tool_Management
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string id = textBox_Email.Text;
            string pw = textBox_Password.Password;

            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("아이디를 입력하세요.");
                return;
            }
            if (string.IsNullOrWhiteSpace(pw))
            {
                MessageBox.Show("비밀번호를 입력하세요.");
                return;
            }
            List<User> users = User.users;
            var user = users.FirstOrDefault(u => u.ID == id);

            if (user == null)
            {
                MessageBox.Show("아이디가 존재하지 않습니다.");
                return;
            }
            string inputPW = HashPW(pw);
            if (user.Hashedpw != inputPW)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.");
                return;
            }
            //Login Success
            PrintHashedPassword(pw);    //Encrpytion Check
        }
        public static string HashPW(string pw)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pw));
                return Convert.ToBase64String(bytes);
            }
        }
        public static void PrintHashedPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hashedString = BitConverter.ToString(hashedBytes).Replace("-", " ");

                MessageBox.Show(hashedString);
            }
        }
        private void SignUpBlock_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow reg = new SignUpWindow();
            reg.Show();
        }
    }
}
