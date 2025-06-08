using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SFS_Tool_Management.Data;
using SFS_Tool_Management.Models;
using SFS_Tool_Management.Views;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace SFS_Tool_Management.ViewModels
{
    public partial class SignInViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? iD;
        [ObservableProperty]
        private string? password;
        [ObservableProperty]
        private UserList? currentUser;
        [RelayCommand]
        private async Task SignIn()
        {
            try
            {
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
                await using (var db = new AppDbContext())
                {
                    var userLog = await db.UserList.FirstOrDefaultAsync(u => u.UserID == ID);
                    if (userLog == null)
                    {
                        MessageBox.Show("존재하지 않는 아이디입니다.", "로그인 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string hashedPassword = Encrypter.HashPW(Password);
                    if (userLog.PasswordHash != hashedPassword)
                    {
                        MessageBox.Show("비밀번호가 틀렸습니다.", "로그인 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    MessageBox.Show($"{userLog.Name}님, 환영합니다.", "로그인 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow mainWindow =  new MainWindow(ID);
                    mainWindow.Show();
                    Application.Current.MainWindow.Close();

                    var authenticatedUser = new UserList(
                        userLog.Name,
                        ID,
                        userLog.Position,
                        userLog.Department,
                        userLog.PhoneNumber,
                        userLog.IsAdmin,
                        hashedPassword);
                    UserList.Instance.SetCurrentUser(authenticatedUser);
                    CurrentUser = authenticatedUser;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"로그인 중 오류가 발생했습니다: {ex.Message}");
            }
        }
        [RelayCommand]
        public void OpenSignUpPage()
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
        }
    }
}
