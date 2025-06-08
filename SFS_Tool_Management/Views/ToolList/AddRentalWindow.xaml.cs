using Microsoft.Data.SqlClient;
using SFS_Tool_Management.Repositories;
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

namespace SFS_Tool_Management.Views.ToolList
{
    /// <summary>
    /// AddRentalWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddRentalWindow : Window
    {
        private Dictionary<string, string> serialConditionMap = new Dictionary<string, string>();
        private readonly string _userID;
        public AddRentalWindow(string toolId, string modelName, string userID)
        {
            InitializeComponent();
            _userID = userID;
            ModelBox.Text = modelName;

            // 사용 가능한 SerialNumber 조회
            string connectionString = SQLRepository.BuildConnectionString();


            string query = "SELECT SerialNumber, Condition FROM ToolInstance WHERE ToolID = @ToolID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ToolID", toolId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string serialNumber = reader["SerialNumber"].ToString();
                        string condition = reader["Condition"].ToString();

                        SerialComboBox.Items.Add(serialNumber);
                        serialConditionMap[serialNumber] = condition;

                    }
                }
            }

            /*using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ToolID", toolId);  // toolId는 string이라고 가정
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SerialComboBox.Items.Add(reader["SerialNumber"].ToString());
                    }
                }
            }*/
        }


        private void Confirm_Click(object sender, RoutedEventArgs e)
        {

            string serialNumber = SerialComboBox.Text;
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                MessageBox.Show("시리얼 번호를 선택해주세요.");
                return;
            }

            if(!serialConditionMap.TryGetValue(serialNumber, out string condition))
            {
                MessageBox.Show("상태 정보를 확인할 수 없습니다.");
                return;
            }

            if(condition != "정상")
            {
                MessageBox.Show($"현재 상태가 [{condition}]이므로 대여할 수 없습니다.");
                return;
            }
            string modelName = ModelBox.Text;
            string purpose = PurposeBox.Text;

            DateTime rentalDate = RentalStartDatePicker.SelectedDate ?? DateTime.Now;
            DateTime returnDate = ReturnDatePicker.SelectedDate ?? DateTime.Now.AddDays(7);

            try
            {
                string connectionString = SQLRepository.BuildConnectionString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                 
                    conn.Open();
              

                    // 1. SerialNumber → ToolID 조회
                    string toolId = "";
                    string findToolIdQuery = "SELECT ToolID FROM ToolInstance WHERE SerialNumber = @SerialNumber";
                    using (SqlCommand findCmd = new SqlCommand(findToolIdQuery, conn))
                    {
                        findCmd.Parameters.AddWithValue("@SerialNumber", serialNumber);
                        object result = findCmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("ToolID를 찾을 수 없습니다.");
                            return;
                        }

                        toolId = result.ToString();
                    }

                    // 2. RentalHistory INSERT
                    string insertQuery = @"
                    INSERT INTO RentalHistory 
                    (RentalID, Purpose, RentalStartDate, PlannedReturnDate, IsReturned, UserID, SerialNumber)
                    VALUES 
                    (@RentalID, @Purpose, @RentalStartDate, @PlannedReturnDate, 0, @UserID, @SerialNumber)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        string rentalId = Guid.NewGuid().ToString();
                        string userId = _userID;

                        cmd.Parameters.AddWithValue("@RentalID", rentalId);
                        cmd.Parameters.AddWithValue("@Purpose", purpose);
   
                        cmd.Parameters.AddWithValue("@RentalStartDate", rentalDate);
                        cmd.Parameters.AddWithValue("@PlannedReturnDate", returnDate);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@SerialNumber", serialNumber);

                        cmd.ExecuteNonQuery(); // ← 반드시 실행해야 저장됨
                    }

                    // 3. Tool 수량 차감 (ToolID 기준)
                    string updateToolQtyQuery = @"
                    UPDATE Tool 
                    SET AvailableQuantity = AvailableQuantity - 1 
                    WHERE ToolID = @ToolID AND AvailableQuantity > 0";

                    using (SqlCommand updateCmd = new SqlCommand(updateToolQtyQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@ToolID", toolId); // toolId는 SerialNumber → ToolID 조회한 결과
                        updateCmd.ExecuteNonQuery();
                    }

                    string updateConditionQuery = @"
                    UPDATE ToolInstance 
                    SET Condition = '대여중' 
                    WHERE SerialNumber = @SerialNumber";

                    using (SqlCommand conditionCmd = new SqlCommand(updateConditionQuery, conn))
                    {
                        conditionCmd.Parameters.AddWithValue("@SerialNumber", serialNumber);
                        conditionCmd.ExecuteNonQuery();
                    }

                }

                MessageBox.Show("대여 요청이 완료되었습니다.");
                this.DialogResult = true;
                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB 처리 중 오류: " + ex.Message);
            }

           
        }

        private void SerialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SerialComboBox.SelectedItem != null)
            {
                string selectedSerial = SerialComboBox.SelectedItem.ToString();

                if (serialConditionMap.TryGetValue(selectedSerial, out string condition))
                {
                    conditionBlock.Text = $"{condition}";
                }
                else
                {
                    conditionBlock.Text = "Condition 정보를 불러올 수 없습니다.";
                }
            }
        } 


        

    }
}
