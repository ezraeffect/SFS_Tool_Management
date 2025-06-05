using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SFS_Tool_Management.Helpers
{
    public partial class ToastService : ObservableObject
    {
        private static readonly ToastService _instance = new();

        public static ToastService Instance => _instance;

        [ObservableProperty]
        private string? message;

        [ObservableProperty]
        private Geometry? icon;

        [ObservableProperty]
        private Brush? color;

        [ObservableProperty]
        private bool isVisible;

        public void Show(string message, Geometry icon, Brush color)
        {
            Message = message;
            Icon = icon;
            Color = color;

            ((MainWindow)Application.Current.MainWindow).OpenToast();
        }
    }
}
