using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SFS_Tool_Management.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        string? toastMessage;

        [ObservableProperty]
        Geometry? toastGeometry;
        
        public void ShowToastView(string msg, Geometry ico)
        {
            
        }

        public void ShowToastView(string msg, string icoPath)
        {
            // 매개변수의 값이 Null이거나 공란이 아니라면,
            if (!string.IsNullOrWhiteSpace(msg) && !string.IsNullOrWhiteSpace(icoPath))
            {
                // Path String을 Geometry 객체로 Parse한다.
                Geometry ico = Geometry.Parse(icoPath);
            }
            
        }

        public void CloseToastView()
        {

        }
    }
}
