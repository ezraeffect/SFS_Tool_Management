using Microsoft.Data.SqlClient;
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
        private string connectionString = @"Server=tcp:***REMOVED***,***REMOVED***;Initial Catalog=Tool;Persist Security Info=False;User ID=***REMOVED***;Password=***REMOVED***;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=90;";


        public ToolListPage()
        {

            InitializeComponent();
            LoadToolData();
            
        }

        private void LoadToolData()
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Tool";

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
                string status = selectedRow["Status"].ToString();

                var editPopup = new EditToolWindow(toolId, toolType, modelName, manufacture,
                    totalQty, availableQty, purchaseDate, durabilityLimit, status);
                editPopup.ShowDialog();
            }

        }

        private void DeleteTool_Click(object sender, RoutedEventArgs e)
        {
            if (ToolDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int id = Convert.ToInt32(selectedRow["Id"]);

                var result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM Tool WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadToolData();
                }
            }
        }

    }
}
