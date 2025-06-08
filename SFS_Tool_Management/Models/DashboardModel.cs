using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFS_Tool_Management.Models
{
    public partial class DashboardModel
    {
        public string? TotalQuantity;
        public string? AvailableQuantity;
        public string? RequiredCheckQuantity;
        public string? ChekcingQuantity;
        public string? RequiredCalibrationQuantity;
        public string? CalibratingQuantity;
        public string? RentalQuantity;
    }

    public partial class RentalToolListModel
    {
        /* 사용자가 대여한 공구 Model */
        public string? SerialNumber { get; set; } // 시리얼 번호
        public string? RentalStartDate { get; set; } // 대여 시작일
        public string? PlannedReturnDate { get; set; } // 반납 예정일
        public string? RemainingDay { get; set; } // 반납까지 남은 날짜
    }

    public partial class DailyRentalStatusModel
    {
        /* 일별 대여/반납 건수 추이 */
        public DateTime? Date { get; set; } // 날짜
        public int? RentalCount { get; set; } // 대여 횟수
        public int? ReturnCount { get; set; } // 반납 횟수
    }

    public partial class ToolStatusModel
    {
        /* 공구 상태별 분포 Model */
        public string? Status { get; set; } // 상태
        public int? Count { get; set; } // 개수
    }
}
