using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using SFS_Tool_Management.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SFS_Tool_Management.Views.Repair
{
    public partial class RepairHistoryPage : Page
    {
        private readonly string _userID;
        private string connectionString = SQLRepository.BuildConnectionString();

        private readonly string defaultQuery = @"SELECT rh.*, ti.ToolID, t.ModelName
                                    FROM RepairHistory rh
                                    JOIN ToolInstance ti ON rh.SerialNumber = ti.SerialNumber
                                    JOIN Tool t ON ti.ToolID = t.ToolID";
        public RepairHistoryPage(string userID)
        {
            InitializeComponent();

            _userID = userID;


            LoadRepairData(defaultQuery, new List<SqlParameter>());
        }



        private void LoadRepairData(string query, List<SqlParameter> parameters)
        {
            string connectionString = SQLRepository.BuildConnectionString();
            DataTable table = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }

            RepairDataGrid.ItemsSource = table.DefaultView;
        }


        // 필터 버튼 클릭
        private void FilterSearch_Click(object sender, RoutedEventArgs e)
        {
            // 1. 공구명
            string modelName = ModelNameFilterBox.Text.Trim();

            // 2. 사용자 ID
            string userID = UserIDBox.Text.Trim();

            // 3. 상태
            string selectedStatus = (ReturnStatusCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;
            
            // 쿼리 작성
            StringBuilder queryBuilder = new StringBuilder(@"
                                                            SELECT rh.*, ti.ToolID, t.ModelName
                                                            FROM RentalHistory rh
                                                            JOIN ToolInstance ti ON rh.SerialNumber = ti.SerialNumber
                                                            JOIN Tool t ON ti.ToolID = t.ToolID
                                                            WHERE 1=1");

            List<SqlParameter> parameters = new List<SqlParameter>();

            // 1. 공구명(modelName) 필터링
            if (!string.IsNullOrEmpty(modelName))
            {
                queryBuilder.Append(" AND t.ModelName LIKE @ModelName");
                parameters.Add(new SqlParameter("@ModelName", "%" + modelName + "%"));
            }

            // 2. 사용자 ID(userID) 필터링
            if (!string.IsNullOrEmpty(userID))
            {
                queryBuilder.Append(" AND rh.UserID LIKE @UserID");
                parameters.Add(new SqlParameter("@userID", "%" + userID + "%"));
            }

            // 3. 상태(status) 필터링
            if (selectedStatus == "대여중")
            {
                queryBuilder.Append(" AND rh.IsReturned = 0");
            }
            if(selectedStatus == "반납완료")
            {
                queryBuilder.Append(" AND rh.IsReturned = 1");

                // 6. 반납 완료일 날짜
                if (ReturnDatePicker.SelectedDate.HasValue)
                {
                    DateTime selectedDate = ReturnDatePicker.SelectedDate.Value.Date;
                    DateTime nextDate = selectedDate.AddDays(1);

                    queryBuilder.Append(" AND rh.RentalEndDate >= @SelectedDate AND rh.RentalEndDate < @NextDate");
                    parameters.Add(new SqlParameter("@SelectedDate", selectedDate));
                    parameters.Add(new SqlParameter("@NextDate", nextDate));
                }
            }

            // 4. 대여일 기준 시작 날짜
            if (startDate.HasValue)
            {
                queryBuilder.Append(" AND rh.RentalStartDate = @StartDate");
                parameters.Add(new SqlParameter("@StartDate", startDate.Value));
            }

            // 5. 대여일 기준 종료 날짜
            if (endDate.HasValue)
            {
                queryBuilder.Append(" AND rh.RentalStartDate = @EndDate");
                parameters.Add(new SqlParameter("@EndDate", endDate.Value));
            }


            LoadRepairData(queryBuilder.ToString(), parameters);
  
        }

        // CSV 내보내기 클릭
        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            if(RepairDataGrid.Items.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV 파일 (*.csv)|*.csv",
                FileName = $"RepairHistory_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var dataView = RepairDataGrid.ItemsSource as DataView;
                    if (dataView == null)
                    {
                        MessageBox.Show("데이터 형식을 인식하지 못했습니다.");
                        return;
                    }

                    DataTable table = dataView.ToTable();

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        sb.Append(table.Columns[i].ColumnName);
                        if (i < table.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();

                    foreach (DataRow row in table.Rows)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            var cell = row[i]?.ToString().Replace(",", " ").Replace("\n", " ").Replace("\r", " ");
                            sb.Append(cell);
                            if (i < table.Columns.Count - 1)
                                sb.Append(",");
                        }
                        sb.AppendLine();
                    }
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("CSV 파일로 내보내기 완료!", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("csv 저장 중 오류 발생:\n" + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                
            }
        }

        private void ClearDateFilters_Click(object sender,RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            ReturnDatePicker.SelectedDate = null;
        }

        private void ConfirmReq_Click(object sender, RoutedEventArgs e)
        {
            /* 기능: 요청 Confirm
            - ToolInstance.Condition 값 변경 점검 중 / 검교정 중 / 수리 중
            - RepairHistory.RepairStartDate 지금 날짜, 시각으로 작성
            - RepairHistory.PerformedBy UserID값으로 작성
            - RepairHistory.RepairType 작성 */

            if (RepairDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if (selectedRow["RepairStartDate"] == DBNull.Value || string.IsNullOrWhiteSpace(selectedRow["RepairStartDate"].ToString()))
                {
                    string RepairType = (string)selectedRow["RepairType"];
                    string Condition = RepairType + " 중";
                    string SerialNumber = selectedRow["SerialNumber"].ToString();
                    string RepairID = selectedRow["RepairID"].ToString();
                    string UserID = _userID;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"UPDATE ToolInstance
                                        SET Condition = @Condition
                                        WHERE SerialNumber = @SerialNumber;";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Condition", Condition);
                            cmd.Parameters.AddWithValue("@SerialNumber", SerialNumber);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = @"UPDATE RepairHistory
                                            SET RepairStartDate = @RepairStartDate,
                                                PerformedBy = @UserID
                                            WHERE RepairID = @RepairID;";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@RepairID", RepairID);
                            cmd.Parameters.AddWithValue("@RepairStartDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UserID", UserID);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"{SerialNumber}의 {RepairType} 요청을 허가하였습니다!", "허가 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRepairData(defaultQuery, new List<SqlParameter>());
                }
                else
                {
                    MessageBox.Show("이미 허가된 요청은 처리가 불가능합니다!");
                }
            }
        }
        private void DoneReq_Click(object sender, RoutedEventArgs e)
        {
            /* 기능 : 요청 Done
            - ToolInstance.Condition 값 변경 정상
            - RepairHistory.Description 값 Textbox받아서 작성
            - RepairHistory.RepairEndDate 지금 날짜, 시각으로 작성 */

            if (RepairDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if (selectedRow["RepairStartDate"] != DBNull.Value && selectedRow["RepairEndDate"] == DBNull.Value)
                {
                    string RepairType = (string)selectedRow["RepairType"];
                    string SerialNumber = selectedRow["SerialNumber"].ToString();
                    string RepairID = selectedRow["RepairID"].ToString();
                    string UserID = _userID;

                    var popup = new DoneRepairWindow(_userID, RepairType, SerialNumber, RepairID);
                    bool? result = popup.ShowDialog();

                    if (result == true)
                    {
                        MessageBox.Show($"{SerialNumber}의 {RepairType} 요청을 허가하였습니다!", "허가 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadRepairData(defaultQuery, new List<SqlParameter>());
                    }
                }
                else
                {
                    MessageBox.Show("요청 완료 조건에 맞지 않아 처리가 불가능합니다!");
                }
            }
        }
    }
}