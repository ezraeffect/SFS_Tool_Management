using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace SFS_Tool_Management
{
    /// <summary>
    /// AddToolWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddToolWindow : Window
    {
        private string connectionString = @"Server=tcp:sfstool.database.windows.net,1433;Initial Catalog=SFS;Persist Security Info=False;User ID=Codingon;Password=sfs2751!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=90;";

        public AddToolWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO ToolList 
                        (ToolID, ToolType, ModelName, Manufacture, TotalQuantity, AvailableQuantity, PurchaseDate, DurabilityLimit, Status)
                        VALUES 
                        (@ToolID, @ToolType, @ModelName, @Manufacture, @TotalQuantity, @AvailableQuantity, @PurchaseDate, @DurabilityLimit, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ToolID", ToolIDTextBox.Text);
                        cmd.Parameters.AddWithValue("@ToolType", ToolTypeTextBox.Text);
                        cmd.Parameters.AddWithValue("@ModelName", ModelNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@Manufacture", ManufactureTextBox.Text);
                        cmd.Parameters.AddWithValue("@TotalQuantity", int.Parse(TotalQuantityTextBox.Text));
                        cmd.Parameters.AddWithValue("@AvailableQuantity", int.Parse(AvailableQuantityTextBox.Text));
                        cmd.Parameters.AddWithValue("@PurchaseDate", PurchaseDatePicker.SelectedDate ?? DateTime.Now);
                        cmd.Parameters.AddWithValue("@DurabilityLimit", DurabilityLimitPicker.SelectedDate ?? DateTime.Now);
                        cmd.Parameters.AddWithValue("@Status", StatusTextBox.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = true; // 부모 창에서 추가 성공 여부 확인
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("공구 추가 중 오류 발생:\n" + ex.Message);
            }
        }
    }
}
