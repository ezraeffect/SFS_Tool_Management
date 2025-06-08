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
using ScottPlot.Plottables;
using SFS_Tool_Management.Repositories;

namespace SFS_Tool_Management.Views.Repair
{
    /// <summary>
    /// ConfirmRepairWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConfirmRepairWindow : Window
    {
        private string? _userID;
        private string? _repairID;
        public ConfirmRepairWindow(string userID, string repairID)
        {
            InitializeComponent();

            _userID = userID;
            _repairID = repairID;

            // Repair 행위 조회
            string connectionString = SQLRepository.BuildConnectionString();
            string query = "SELECT SerialNumber, UserID, ReportedDate FROM dbo.RepairHistory WHERE RepairID = @repairID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@repairID", _repairID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string serialNumber = reader["SerialNumber"].ToString();
                            string reqUserID = reader["UserID"].ToString();
                            string ReportedDate = reader["ReportedDate"].ToString();
                            // 이후 로직
                        }
                        else
                        {
                            MessageBox.Show("해당 수리 내역이 존재하지 않습니다.", "오류");
                        }
                    }
                }
            }
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Job_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
