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
using Microsoft.Data.SqlClient;
using ScottPlot.Statistics;
using SFS_Tool_Management.Repositories;

namespace SFS_Tool_Management.Views.Repair
{
    /// <summary>
    /// DoneRepairWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DoneRepairWindow : Window
    {
        private string _userID;
        private string _repairType;
        private string _serialNumber;
        private string _repairID;

        private string connectionString = SQLRepository.BuildConnectionString();

        public DoneRepairWindow(string UserID, string RepairType, string SerialNumber, string RepairID)
        {
            InitializeComponent();

            _userID = UserID;
            _repairType = RepairType;
            _serialNumber = SerialNumber;
            _repairID = RepairID;


            RepairID_TextBlock.Text = RepairID;
            RepairType_TextBlock.Text = RepairType;
            SerialNumber_TextBlock.Text = SerialNumber;
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            /* 기능 : 요청 Done
            - ToolInstance.Condition 값 변경 정상
            - RepairHistory.Description 값 Textbox받아서 작성
            - RepairHistory.RepairEndDate 지금 날짜, 시각으로 작성 */
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE ToolInstance
                                            SET Condition = @Condition
                                            WHERE SerialNumber = @SerialNumber;";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Condition", "정상");
                        cmd.Parameters.AddWithValue("@SerialNumber", _serialNumber);

                        cmd.ExecuteNonQuery();
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE RepairHistory
                                                SET RepairEndDate = @RepairEndDate,
                                                    Description = @Description
                                                WHERE RepairID = @RepairID;";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RepairID", _repairID);
                        cmd.Parameters.AddWithValue("@RepairEndDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Description", Description_TextBox.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = true; // 부모 창에서 성공 여부 확인
                this.Close();

                }
            catch (Exception ex)
            {
                MessageBox.Show("요청 완료 중 오류 발생:\n" + ex.Message);
            }
        }
    }
}
