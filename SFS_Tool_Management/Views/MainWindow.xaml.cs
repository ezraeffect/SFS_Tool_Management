﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SFS_Tool_Management.Models;
using SFS_Tool_Management.ViewModels;
using SFS_Tool_Management.Views;

namespace SFS_Tool_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string userID)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(MainFrame, userID);
        }
    }
}