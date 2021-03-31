using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffBookHolidaysViewModel
    {

        public int BookHolidayId{ get; set; }
        public int TotalNoOfHolidays { get; set; }

        public int NoOfHolidaysRequested { get; set; }
        public int NoOfHolidaysLeft { get; set; }
        public string HolidaysStartDate { get; set; }
        public string HolidaysEndDate { get; set; }

        public int NoOfHolidaysTaken { get; set; }
        public DB.HollidayRequestStatus HolidaysRequestStatus { get; set; }
    }
}