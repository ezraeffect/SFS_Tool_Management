using SFS_Tool_Management.Models;
using System;
using System.Collections.Generic;
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
using Microsoft.Data.SqlClient;
using System.Data;

namespace SFS_Tool_Management.Views
{
    /// <summary>
    /// FunctionTestPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FunctionTestPage : Page
    {
        public SQLcInfo cInfo = new SQLcInfo() {
            Server = "***REMOVED***",
            Port = "***REMOVED***",
            Database = "SFS",
            Uid = "***REMOVED***",
            PWD = "***REMOVED***"
        };

        public FunctionTestPage()
        {
            InitializeComponent();
        }

        private void button_ConnectSQL_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new(cInfo.GetConnectInfo());
            conn.Open();

            string query = "SELECT * FROM Persons";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ReadSingleRow(reader);
                }
            }
        }
        public void ReadSingleRow(IDataRecord record)
        {
            textBlock_debug.Text += String.Format("{0}, {1}", record[0], record[1]);
            textBlock_debug.Text += "\r\n";
        }
    }
}
