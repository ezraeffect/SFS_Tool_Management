using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using SFS_Tool_Management.Repositories;

namespace SFS_Tool_Management
{
    public partial class EditToolWindow : Window
    {
        public bool IsUpdated { get; set; } = false;

        private string connectionString = SQLRepository.BuildConnectionString();

        private string ToolID;

        public EditToolWindow(string toolId, string toolType, string modelName, string manufacture,
                              int totalQty, int availableQty, DateTime purchaseDate,
                              DateTime durabilityLimit)
        {
            InitializeComponent();

            // 필드 초기화
            ToolID = toolId;
            ToolIDTextBox.Text = toolId;
            ToolIDTextBox.IsEnabled = false; // 수정 불가

            ToolTypeTextBox.Text = toolType;
            ModelNameTextBox.Text = modelName;
            ManufactureTextBox.Text = manufacture;
            TotalQuantityTextBox.Text = totalQty.ToString();
            AvailableQuantityTextBox.Text = availableQty.ToString();
            PurchaseDatePicker.SelectedDate = purchaseDate;
            DurabilityLimitPicker.SelectedDate = durabilityLimit;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE ToolList SET
                            ToolType = @ToolType,
                            ModelName = @ModelName,
                            Manufacture = @Manufacture,
                            TotalQuantity = @TotalQuantity,
                            AvailableQuantity = @AvailableQuantity,
                            PurchaseDate = @PurchaseDate,
                            DurabilityLimit = @DurabilityLimit,
                        WHERE ToolID = @ToolID";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ToolID", ToolID);
                        cmd.Parameters.AddWithValue("@ToolType", ToolTypeTextBox.Text);
                        cmd.Parameters.AddWithValue("@ModelName", ModelNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@Manufacture", ManufactureTextBox.Text);
                        cmd.Parameters.AddWithValue("@TotalQuantity", int.Parse(TotalQuantityTextBox.Text));
                        cmd.Parameters.AddWithValue("@AvailableQuantity", int.Parse(AvailableQuantityTextBox.Text));
                        cmd.Parameters.AddWithValue("@PurchaseDate", PurchaseDatePicker.SelectedDate ?? DateTime.Now);
                        cmd.Parameters.AddWithValue("@DurabilityLimit", DurabilityLimitPicker.SelectedDate ?? DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                IsUpdated = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("수정 중 오류 발생:\n" + ex.Message);
            }
        }
    }
}
