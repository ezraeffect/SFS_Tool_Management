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
    /// AddToolInstanceWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddToolInstanceWindow : Window
    {
        private readonly string connectionString = SQLRepository.BuildConnectionString();

        public AddToolInstanceWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string serialNumber = SerialNumberBox.Text.Trim();
            string toolId = ToolIDBox.Text.Trim();
            string condition = (ConditionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            DateTime? lastCalDate = LastCalDatePicker.SelectedDate;
            DateTime? nextCalDate = NextCalDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(toolId) || string.IsNullOrWhiteSpace(condition))
            {
                MessageBox.Show("모든 필드를 입력해주세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Tool 테이블에 해당 ToolID가 있는지 확인
                    string checkToolQuery = "SELECT COUNT(*) FROM Tool WHERE ToolID = @ToolID";
                    using (SqlCommand checkCmd = new SqlCommand(checkToolQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@ToolID", toolId);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                        {
                            // ToolID 없으면 신규 Tool 추가 (기본값으로)
                            string insertToolQuery = @"
    INSERT INTO Tool 
    (ToolID, ToolType, ModelName, Manufacture, TotalQuantity, AvailableQuantity, PurchaseDate, DurabilityLimit, Status)
    VALUES 
    (@ToolID, 'Unknown', 'Unknown', 'Unknown', 0, 0, NULL, NULL, '추가됨')";

                            using (SqlCommand insertToolCmd = new SqlCommand(insertToolQuery, conn))
                            {
                                insertToolCmd.Parameters.AddWithValue("@ToolID", toolId);
                                insertToolCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // 2. ToolInstance INSERT
                    string insertInstanceQuery = @"INSERT INTO ToolInstance (SerialNumber, LastCalibrationDate, NextCalibrationDate, Condition, ToolID)
                                                   VALUES (@SerialNumber, @LastCal, @NextCal, @Condition, @ToolID)";

                    using (SqlCommand cmd = new SqlCommand(insertInstanceQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SerialNumber", serialNumber);
                        cmd.Parameters.AddWithValue("@LastCal", (object?)lastCalDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NextCal", (object?)nextCalDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Condition", condition);
                        cmd.Parameters.AddWithValue("@ToolID", toolId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("ToolInstance가 추가되었습니다.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
