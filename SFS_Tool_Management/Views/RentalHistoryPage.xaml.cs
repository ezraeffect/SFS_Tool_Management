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

namespace SFS_Tool_Management.Views
{
    public partial class RentalHistoryPage : Page
    {
        private string connectionString = SQLRepository.BuildConnectionString();

        public RentalHistoryPage()
        {
            InitializeComponent();

            string defaultQuery = @"
        SELECT rh.*, ti.ToolID, t.ModelName
        FROM RentalHistory rh
        JOIN ToolInstance ti ON rh.SerialNumber = ti.SerialNumber
        JOIN Tool t ON ti.ToolID = t.ToolID
    ";
            LoadRentalData(defaultQuery, new List<SqlParameter>());
        }



        private void LoadRentalData(string query, List<SqlParameter> parameters)
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

            RentalDataGrid.ItemsSource = table.DefaultView;
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


            LoadRentalData(queryBuilder.ToString(), parameters);
  
        }

        // CSV 내보내기 클릭
        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            if(RentalDataGrid.Items.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV 파일 (*.csv)|*.csv",
                FileName = $"RentalHistory_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var dataView = RentalDataGrid.ItemsSource as DataView;
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

        private void ReturnTool_Click(object sender, RoutedEventArgs e)
        {
            if (RentalDataGrid.SelectedItem is DataRowView row)
            {
                if (row["RentalEndDate"] != DBNull.Value)
                {
                    MessageBox.Show("이미 반납된 항목입니다.");
                    return;
                }

                // SerialNumber는 문자열이므로 그대로 string으로 받기
                string serialNumber = row["SerialNumber"].ToString();

                // 반납 처리 메서드 호출
                ReturnTool(serialNumber);
            }
        }


        private void ReturnTool(string serialNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                -- 1. ToolInstance 상태 변경
                UPDATE ToolInstance
                SET Condition = '정상'
                WHERE SerialNumber = @SerialNumber;

                -- 2. RentalHistory 반납 처리
                UPDATE RentalHistory
                SET RentalEndDate = GETDATE(), IsReturned = 1
                WHERE SerialNumber = @SerialNumber AND RentalEndDate IS NULL;

                -- 3. Tool의 AvailableQuantity 증가
                UPDATE Tool
                SET AvailableQuantity = AvailableQuantity + 1
                WHERE ToolID = (
                    SELECT ToolID
                    FROM ToolInstance
                    WHERE SerialNumber = @SerialNumber
                );
            ";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SerialNumber", serialNumber);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("반납이 완료되었습니다.");

                // 목록 다시 불러오기
                string defaultQuery = @"
                                    SELECT rh.*, ti.ToolID, t.ModelName
                                    FROM RentalHistory rh
                                    JOIN ToolInstance ti ON rh.SerialNumber = ti.SerialNumber
                                    JOIN Tool t ON ti.ToolID = t.ToolID
                                    ";
                LoadRentalData(defaultQuery, new List<SqlParameter>());

            }
            catch (Exception ex)
            {
                MessageBox.Show("반납 처리 중 오류: " + ex.Message);
            }
        }
        private void ClearDateFilters_Click(object sender,RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            ReturnDatePicker.SelectedDate = null;
        }

    }
}