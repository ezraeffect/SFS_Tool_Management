using Microsoft.Data.SqlClient;
using Microsoft.Win32;
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
        private string connectionString = @"Server=tcp:***REMOVED***,***REMOVED***;
                                            Initial Catalog=Tool;
                                            Persist Security Info=False;
                                            User ID=***REMOVED***;
                                            Password=***REMOVED***;
                                            MultipleActiveResultSets=False;
                                            Encrypt=True;
                                            TrustServerCertificate=False;
                                            Connection Timeout=90;";

        public RentalHistoryPage()
        {
            InitializeComponent();
            LoadRentalData();
        }

        private void LoadRentalData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM dbo.RentalHistory";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    RentalDataGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("대여기록 불러오기 오류: " + ex.Message);
            }
        }

        // 필터 버튼 클릭 (아직 구현 전)
        private void FilterSearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("필터 검색은 다음 단계에서 구현할 예정입니다.");
        }

        // CSV 내보내기 클릭 (다음 단계에서 구현)
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

                // ✅ SerialNumber는 문자열이므로 그대로 string으로 받기
                string serialNumber = row["SerialNumber"].ToString();

                // ✅ 반납 처리 메서드 호출
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
                SET Condition = '사용가능'
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
                LoadRentalData(); // 목록 갱신
            }
            catch (Exception ex)
            {
                MessageBox.Show("반납 처리 중 오류: " + ex.Message);
            }
        }




    }
}