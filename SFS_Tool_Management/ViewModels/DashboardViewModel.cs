using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using SFS_Tool_Management.Repositories;
using System.Reflection.PortableExecutable;
using CommunityToolkit.Mvvm.Input;
using System.Windows;


namespace SFS_Tool_Management.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private DashboardModel _model;

        public DashboardViewModel()
        {
            _model = new DashboardModel();
        }

        [ObservableProperty]
        public string? totalQuantity;

        [ObservableProperty]
        public string? availableQuantity;

        [ObservableProperty]
        public string? requiredCheckQuantity;

        [ObservableProperty]
        public string? chekcingQuantity;

        [ObservableProperty]
        public string? requiredCalibrationQuantity;

        [ObservableProperty]
        public string? calibratingQuantity;

        [ObservableProperty]
        public string? rentalQuantity;

        public void LoadData(DashboardModel model)
        {
            TotalQuantity = model.TotalQuantity;
            AvailableQuantity = model.AvailableQuantity;
            RequiredCheckQuantity = model.RequiredCheckQuantity;
            ChekcingQuantity = model.ChekcingQuantity;
            RequiredCalibrationQuantity = model.RequiredCalibrationQuantity;
            CalibratingQuantity = model.CalibratingQuantity;
            RentalQuantity = model.RentalQuantity;
        }

        public DashboardModel ToModel()
        {
            return new DashboardModel
            {
                TotalQuantity = TotalQuantity,
                AvailableQuantity = AvailableQuantity,
                RequiredCheckQuantity = RequiredCheckQuantity,
                ChekcingQuantity = ChekcingQuantity,
                RequiredCalibrationQuantity = RequiredCalibrationQuantity,
                CalibratingQuantity = CalibratingQuantity,
                RentalQuantity = RentalQuantity,
            };
        }

        [RelayCommand]
        public async Task GetDataAsync()
        {
            var repo = new SQLRepository();

            string query = "SELECT COUNT(*) AS TotalQuantity, "
                        + "COUNT(CASE WHEN Condition = '정상' THEN 1 END) AS AvailableQuantity, "
                        + "COUNT(CASE WHEN Condition = '점검 필요' THEN 1 END) AS RequiredCheckQuantity, "
                        + "COUNT(CASE WHEN Condition = '점검 중' THEN 1 END) AS ChekcingQuantity, "
                        + "COUNT(CASE WHEN Condition = '교정 필요' THEN 1 END) AS RequiredCalibrationQuantity, "
                        + "COUNT(CASE WHEN Condition = '교정 중' THEN 1 END) AS CalibratingQuantity, "
                        + "COUNT(CASE WHEN Condition = '대여' THEN 1 END) AS RentalQuantity "
                        + "FROM ToolInstance;";

            var result = await repo.ExecuteQueryAsync(query, reader => new DashboardModel
            {
                TotalQuantity = reader.GetInt32(0).ToString(),
                AvailableQuantity = reader.GetInt32(1).ToString(),
                RequiredCheckQuantity = reader.GetInt32(2).ToString(),
                ChekcingQuantity = reader.GetInt32(3).ToString(),
                RequiredCalibrationQuantity = reader.GetInt32(4).ToString(),
                CalibratingQuantity = reader.GetInt32(5).ToString(),
                RentalQuantity = reader.GetInt32(6).ToString()
            });

            if (result.Count > 0)
                LoadData(result[0]);
        }

        [RelayCommand]
        public static void InsertUserInfo()
        {
            var repo = new SQLRepository();
            repo.InsertTestQuery();
        }
    }
}
