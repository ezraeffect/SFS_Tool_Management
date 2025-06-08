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
using SFS_Tool_Management.Data;
using Microsoft.EntityFrameworkCore;
using OpenTK.Graphics.ES11;
using System.Windows.Media;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Windows.Data;


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

        [ObservableProperty]
        private string strName = string.Empty;
        [ObservableProperty]
        private string imageName = string.Empty;
        [ObservableProperty]
        private object? imageBrushed;

        private byte[]? imageData;

        [RelayCommand]
        private async Task SignUp()
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
            await using (var db = new AppDbContext())
            {
                if (await db.UserList.AnyAsync(u => u.UserID == ID))
                {
                    MessageBox.Show("이미 존재하는 사용자 ID입니다. 다른 ID를 선택해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
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

                UserList newUser = new UserList(Name, ID, Position, Department, PhoneNumber, ac, hashedPw, imageData);
                await db.UserList.AddAsync(newUser);
                await db.SaveChangesAsync();

                MessageBox.Show("회원가입이 완료되었습니다.", "회원가입 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private void BrowseFile()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileDialog.Filter = "Image Files (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";

            if (fileDialog.ShowDialog() == true)
            {
                StrName = fileDialog.SafeFileName;
                ImageName = fileDialog.FileName;

                if (GetFileSize(ImageName) > 0 && GetFileSize(ImageName) <= 1_048_576)
                {
                    var isc = new ImageSourceConverter();
                    ImageBrushed = isc.ConvertFromString(ImageName);
                    imageData = ConvertImageToByteArray(ImageName);
                }
                else
                {
                    MessageBox.Show("파일 크기가 1MB를 초과합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    StrName = string.Empty;
                    ImageName = string.Empty;
                }
            }
        }

        public static byte[]? ConvertImageToByteArray(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read); // 읽기 전용으로 FileStream 생성
                var imgByteArr = new byte[fs.Length]; // FileStream 크기 만큼의 Byte 배열 생성
                fs.Read(imgByteArr, 0, (int)fs.Length); // 0부터 FileStream 크기 까지 순차적으로 Byte 배열에 저장
                return imgByteArr; // FileStream 닫기
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public long GetFileSize(string filePath)
        {
            long fileSize = 0;
            if (File.Exists(filePath))
            {
                FileInfo info = new FileInfo(filePath);
                fileSize = info.Length;
            }

            return fileSize;
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
