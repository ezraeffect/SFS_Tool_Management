using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Views.DashboardPage());
            DataContext = new UserViewModel();
        }
        private void button_ShowDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.DashboardPage());
        }

        private void button_ShowToolList_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.ToolListPage());
        }

        private void button_ShowRentalHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.RentalHistoryPage());
        }
        private void button_ShowAdminOption_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.AdminOptionPage());
        }
    }
}