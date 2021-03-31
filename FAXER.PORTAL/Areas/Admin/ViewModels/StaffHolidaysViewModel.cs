using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class StaffHolidaysViewModel
    {

        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        public string City { get; set; }
        public int NoOfDays { get; set; }
        public string StartDate { get; set; }
        public HollidayRequestStatus HolidayStatus { get; set; }
        public string FinishDate { get; set; }
        public string ApprovedByName{ get; set; }
        public int NoTaken { get; set; }
        public int NoLeft { get; set; }
        public int Entitled { get; set; }
    }
}