using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddStaffNoticeViewModel
    {
        public const string BindProperty = "Id , Country ,City , StaffId,CountryName, StaffName, Headline,FullNotice ,DateAndTime";

        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int StaffId { get; set; }
        public string CountryName { get; set; }
        public string StaffName { get; set; }
        public string Headline { get; set; }
        public string FullNotice { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}