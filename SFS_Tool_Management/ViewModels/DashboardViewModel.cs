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
        private string? totalToolCount;

        [ObservableProperty]
        private string? availableToolCount;

        [ObservableProperty]
        private string? repairingToolCount;

        [ObservableProperty]
        private string? rentedToolCount;

        public void LoadData(DashboardModel model)
        {
            TotalToolCount = model.TotalToolCount;
            AvailableToolCount = model.AvailableToolCount;
            RepairingToolCount = model.RepairingToolCount;
            RentedToolCount = model.RentedToolCount;
        }

        public DashboardModel ToModel()
        {
            return new DashboardModel
            {
                TotalToolCount = TotalToolCount,
                AvailableToolCount = AvailableToolCount,
                RepairingToolCount = RepairingToolCount,
                RentedToolCount = RentedToolCount
            };
        }

        [RelayCommand]
        public async Task GetDataAsync()
        {
            var repo = new SQLRepository();
            var result = await repo.ExecuteQueryAsync("SELECT COUNT(*) FROM dbo.Tool", reader => new DashboardModel
            {
                TotalToolCount = reader.GetInt32(0).ToString()
            });

            if (result.Count > 0)
                LoadData(result[0]);
        }
    }
}
