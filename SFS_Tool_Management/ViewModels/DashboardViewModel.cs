using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using CommunityToolkit.Mvvm.ComponentModel;

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
            totalToolCount = model.totalToolCount;
            availableToolCount = model.availableToolCount;
            repairingToolCount = model.repairingToolCount;
            rentedToolCount = model.rentedToolCount;
        }

        public DashboardModel ToModel()
        {
            return new DashboardModel
            {
                totalToolCount = totalToolCount,
                availableToolCount = availableToolCount,
                repairingToolCount = repairingToolCount,
                rentedToolCount = rentedToolCount
            };
        }
    }
}
