using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS_Tool_Management.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using SFS_Tool_Management.Repositories;
using System.Reflection.PortableExecutable;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ScottPlot.WPF;
using System.Data;
using ScottPlot;
using ScottPlot.Plottables;


namespace SFS_Tool_Management.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private DashboardModel _model;
        public WpfPlot DailyToolPlotControl { get; } = new WpfPlot();
        public WpfPlot ToolStautsPlotControl { get; } = new WpfPlot();


        private readonly string _currentID;

        public DashboardViewModel()
        {
            _model = new DashboardModel();
        }
        public DashboardViewModel(string currentID)
        {
            _model = new DashboardModel();
            _currentID = currentID;
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

        [ObservableProperty]
        private ObservableCollection<RentalToolListModel> rentalToolList = new();

        [ObservableProperty]
        private ObservableCollection<RentalToolListModel> overdueToolList = new();

        private List<DateTime> dailyDateTimes = new List<DateTime>();
        private List<int> rentalCounts = new List<int>();
        private List<int> returnCounts = new List<int>();
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

            string query = @"SELECT COUNT(*) AS TotalQuantity,
                            COUNT(CASE WHEN Condition = '정상' THEN 1 END) AS AvailableQuantity,
                            COUNT(CASE WHEN Condition = '점검 필요' THEN 1 END) AS RequiredCheckQuantity,
                            COUNT(CASE WHEN Condition = '점검 중' THEN 1 END) AS ChekcingQuantity,
                            COUNT(CASE WHEN Condition = '교정 필요' THEN 1 END) AS RequiredCalibrationQuantity,
                            COUNT(CASE WHEN Condition = '교정 중' THEN 1 END) AS CalibratingQuantity,
                            COUNT(CASE WHEN Condition = '대여' THEN 1 END) AS RentalQuantity
                            FROM ToolInstance;";

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
        public async Task GetRentalDataAsync()
        {
            /*
             - 반납 예정 공구
             - 1. 사용자 본인의
             - 2. IsReturned가 0이며 (반납되지 않음)
             - 3. PlannedReturnDate가 지금 일자보다 많을때
             - 시리얼 번호, 대여 시작일, 반납 예정일, 남은 날짜
             */

            var repo = new SQLRepository();

            string query = @"SELECT SerialNumber, RentalStartDate, PlannedReturnDate, DATEDIFF(DAY, GETDATE(), PlannedReturnDate)
                            FROM dbo.RentalHistory
                            WHERE UserID = @UserID
                            AND PlannedReturnDate >= GETDATE()
                            AND IsReturned = 0;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserID",  _currentID}
            };

            var result = await repo.ExecuteQueryAsync(query, reader => new RentalToolListModel
            {
                SerialNumber = reader.GetString(0),
                RentalStartDate = reader.GetDateTime(1).ToString(),
                PlannedReturnDate = reader.GetDateTime(2).ToString(),
                RemainingDay = reader.GetInt32(3).ToString()
            }, parameters);

            foreach (var item in result)
                RentalToolList.Add(item);
        }

        [RelayCommand]
        public async Task GetOverdueDataAsync()
        {
            /*
            - 반납 연체 공구
            - 1. 사용자 본인의
            - 2. IsReturned가 0이며 (반납되지 않음)
            - 3. PlannedReturnDate가 지금 일자보다 적을때
            - 시리얼 번호, 대여 시작일, 반납 예정일, 지난 날짜
             */

            var repo = new SQLRepository();

            string query = @"SELECT SerialNumber, RentalStartDate, PlannedReturnDate, DATEDIFF(DAY, PlannedReturnDate, GETDATE()) AS '연체 일자'
	                        FROM dbo.RentalHistory
	                        WHERE UserID = @UserID
	                        AND PlannedReturnDate < GETDATE()
	                        AND IsReturned = 0;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserID",  _currentID}
            };

            var result = await repo.ExecuteQueryAsync(query, reader => new RentalToolListModel
            {
                SerialNumber = reader.GetString(0),
                RentalStartDate = reader.GetDateTime(1).ToString(),
                PlannedReturnDate = reader.GetDateTime(2).ToString(),
                RemainingDay = reader.GetInt32(3).ToString()
            }, parameters);

            foreach (var item in result)
                OverdueToolList.Add(item);
        }

        [RelayCommand]
        public static void InsertUserInfo()
        {
            var repo = new SQLRepository();
            repo.InsertTestQuery();
        }

        public async Task GetDailyToolStatusDataAsync()
        {
            /*
            - 일별 대여/반납 건수 추이 (최근 30일)
            - RentalHistory의 대여일/반납일 기준 일별 집계
             */

            var repo = new SQLRepository();

            string query = @"WITH DateSeries AS (
                                SELECT CAST(GETDATE() - 29 AS DATE) AS [Date]
                                UNION ALL
                                SELECT DATEADD(DAY, 1, [Date])
                                FROM DateSeries
                                WHERE [Date] < CAST(GETDATE() AS DATE)
                            ),
                            RentalCount AS (
                                SELECT CAST(RentalStartDate AS DATE) AS [Date], COUNT(*) AS RentalCount
                                FROM RentalHistory
                                WHERE RentalStartDate >= CAST(GETDATE() - 29 AS DATE)
                                GROUP BY CAST(RentalStartDate AS DATE)
                            ),
                            ReturnCount AS (
                                SELECT CAST(RentalEndDate AS DATE) AS [Date], COUNT(*) AS ReturnCount
                                FROM RentalHistory
                                WHERE RentalEndDate IS NOT NULL AND RentalEndDate >= CAST(GETDATE() - 29 AS DATE)
                                GROUP BY CAST(RentalEndDate AS DATE)
                            )
                            SELECT
                                D.[Date],
                                ISNULL(R.RentalCount, 0) AS RentalCount,
                                ISNULL(T.ReturnCount, 0) AS ReturnCount
                            FROM DateSeries D
                            LEFT JOIN RentalCount R ON D.[Date] = R.[Date]
                            LEFT JOIN ReturnCount T ON D.[Date] = T.[Date]
                            ORDER BY D.[Date]
                            OPTION (MAXRECURSION 0);";

            var result = await repo.ExecuteQueryAsync(query, reader => new DailyRentalStatusModel
            {
                Date = reader.GetDateTime(0),
                RentalCount = reader.GetInt32(1),
                ReturnCount = reader.GetInt32(2)
            });

            foreach (var item in result)
            {
                dailyDateTimes.Add((DateTime)item.Date);
                rentalCounts.Add((int)item.RentalCount);
                returnCounts.Add((int)item.ReturnCount);
            }
        }

        [RelayCommand]
        public async Task SetPlotValue()
        {
            await GetDailyToolStatusDataAsync();
            var rentalCountPlot = DailyToolPlotControl.Plot.Add.Scatter(dailyDateTimes.ToArray(), rentalCounts.ToArray());
            rentalCountPlot.LegendText = "Rented Count";
            var returnCountPlot = DailyToolPlotControl.Plot.Add.Scatter(dailyDateTimes.ToArray(), returnCounts.ToArray());
            returnCountPlot.LegendText = "Returned Count";

            DailyToolPlotControl.Plot.Font.Automatic();
            DailyToolPlotControl.Plot.Axes.DateTimeTicksBottom();
            DailyToolPlotControl.Refresh();
        }
    }
}
