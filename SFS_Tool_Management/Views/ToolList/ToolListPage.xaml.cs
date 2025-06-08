using Microsoft.Data.SqlClient;
using SFS_Tool_Management.Views.ToolList;
using SFS_Tool_Management.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// <summary>
    /// ToolListPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToolListPage : Page
    {
        private string connectionString = SQLRepository.BuildConnectionString();
        
        private DataRowView _lastDeletedTool = null;

        private readonly string _userID;

        public ToolListPage(string userID)
        {

            InitializeComponent();
            LoadToolData();
            _userID = userID;
            
        }

        private void LoadToolData()
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //string query = @"SELECT * FROM Tool";

                    string query = @"SELECT
                                        T.ToolID,
                                        T.ToolType,
                                        T.ModelName,
                                        T.Manufacture,
                                        T.TotalQuantity,
                                        T.AvailableQuantity,
                                        T.PurchaseDate,
                                        T.DurabilityLimit,
                                        I.SerialNumber,
                                        I.LastCalibrationDate,
                                        I.NextCalibrationDate,
                                        I.Condition
                                    FROM
                                        Tool T
                                    INNER JOIN
                                        ToolInstance I ON T.ToolID = I.ToolID;";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ToolDataGrid.ItemsSource = dataTable.DefaultView;

                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("데이터를 불러오는 중 오류 발생:\n" + ex.Message);
            }
        }

        private void AddToolButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = new AddToolWindow();
            bool? result = popup.ShowDialog();

            if (result == true)
            {
                LoadToolData(); // 팝업창에서 삽입이 성공적으로 끝났을 경우만
            }
        }
        private void EditTool_Click(object sender, RoutedEventArgs e)
        {
            if (ToolDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string toolId = selectedRow["ToolID"].ToString();
                string toolType = selectedRow["ToolType"].ToString();
                string modelName = selectedRow["ModelName"].ToString();
                string manufacture = selectedRow["Manufacture"].ToString();
                int totalQty = Convert.ToInt32(selectedRow["TotalQuantity"]);
                int availableQty = Convert.ToInt32(selectedRow["AvailableQuantity"]);
                DateTime purchaseDate = Convert.ToDateTime(selectedRow["PurchaseDate"]);
                DateTime durabilityLimit = Convert.ToDateTime(selectedRow["DurabilityLimit"]);

                var editPopup = new EditToolWindow(toolId, toolType, modelName, manufacture,
                    totalQty, availableQty, purchaseDate, durabilityLimit);

                bool? result = editPopup.ShowDialog();

                // 수정 완료 후 목록 갱신
                if (result == true && editPopup.IsUpdated)
                {
                    /*MessageBox.Show("수정됨 → 데이터 갱신 시작");*/
                    LoadToolData();
                }
            }
        }


        private void DeleteTool_Click(object sender, RoutedEventArgs e)
        {
            if (ToolDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string toolId = selectedRow["ToolID"].ToString();


                var result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _lastDeletedTool = selectedRow;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM Tool WHERE ToolId = @ToolId";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@ToolId", toolId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadToolData();
                }
            }
        }

        private void RestoreTool_Click(object sender, RoutedEventArgs e)
        {
            if (_lastDeletedTool == null)
            {
                MessageBox.Show("복원할 삭제 기록이 없습니다.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Tool 
                                (ToolID, ToolType, ModelName, Manufacture, TotalQuantity, AvailableQuantity, PurchaseDate, DurabilityLimit)
                                VALUES
                                (@ToolID, @ToolType, @ModelName, @Manufacture, @TotalQuantity, @AvailableQuantity, @PurchaseDate, @DurabilityLimit)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ToolID", _lastDeletedTool["ToolID"]);
                    cmd.Parameters.AddWithValue("@ToolType", _lastDeletedTool["ToolType"]);
                    cmd.Parameters.AddWithValue("@ModelName", _lastDeletedTool["ModelName"]);
                    cmd.Parameters.AddWithValue("@Manufacture", _lastDeletedTool["Manufacture"]);
                    cmd.Parameters.AddWithValue("@TotalQuantity", _lastDeletedTool["TotalQuantity"]);
                    cmd.Parameters.AddWithValue("@AvailableQuantity", _lastDeletedTool["AvailableQuantity"]);
                    cmd.Parameters.AddWithValue("@PurchaseDate", _lastDeletedTool["PurchaseDate"]);
                    cmd.Parameters.AddWithValue("@DurabilityLimit", _lastDeletedTool["DurabilityLimit"]);

                    cmd.ExecuteNonQuery();
                }
            }

            _lastDeletedTool = null;
            LoadToolData();
            MessageBox.Show("복원이 완료되었습니다.");
        }


        private void RequestRental_Click(object sender, RoutedEventArgs e)
        {
            if(ToolDataGrid.SelectedItem is DataRowView selectedRow)
            {
                string serialNumber = selectedRow["ToolID"].ToString();
                string modelName = selectedRow["ModelName"].ToString();

                var rentalWindow = new AddRentalWindow(serialNumber, modelName, _userID);
                rentalWindow.ShowDialog();

            }
            else
            {
                MessageBox.Show("공구를 먼저 선택하세요.");
            }

        }

        private void RequestCheck_Click(object sender, RoutedEventArgs e)
        {
            Request("점검 필요", "점검");
        }

        private void RequestCali_Click(object sender, RoutedEventArgs e)
        {
            Request("검교정 필요", "검교정");
        }
        private void RequestFix_Click(object sender, RoutedEventArgs e)
        {
            Request("수리 필요", "수리");
        }


        private void Request(string Condition, string RepairType)
        {
            /* 기능 : 점검, 검교정, 수리 요청 
            - ToolInstance.Condition 값 변경
            - RepairHistory.RepairID 랜덤 값으로 작성
            - RepairHistory.ReportedDate 지금 날짜, 시각으로 작성
            - RepairHistory.UserID 작성
            - RepairHistory.SerialNumber 작성
             */

            if (ToolDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if ((string)selectedRow["Condition"] == "정상")
                {
                    string RepairID = Guid.NewGuid().ToString();
                    string UserID = _userID;
                    string SerialNumber = selectedRow["SerialNumber"].ToString();

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

                        string query = @"INSERT INTO RepairHistory 
                                (RepairID, ReportedDate, UserID, SerialNumber, RepairType)
                                VALUES
                                (@RepairID, @ReportedDate, @UserID, @SerialNumber, @RepairType)";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@RepairID", RepairID);
                            cmd.Parameters.AddWithValue("@ReportedDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@SerialNumber", SerialNumber);
                            cmd.Parameters.AddWithValue("@RepairType", RepairType);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"{SerialNumber}의 {Condition} 요청이 완료되었습니다!", "요청 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadToolData();

                }
                else
                {
                    MessageBox.Show("상태가 정상이 아닌 경우 요청을 할 수 없습니다!");
                }


            }
            else
            {
                MessageBox.Show("공구를 먼저 선택하세요.");
            }
        }
    }
}
