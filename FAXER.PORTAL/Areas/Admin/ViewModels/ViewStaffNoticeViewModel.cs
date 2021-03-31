using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewStaffNoticeViewModel
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Headline { get; set; }

       
        public string Country { get; set; }
        public string City { get; set; }
        public int? StaffId { get; set; }
        public string StaffName { get; set; }
        public string CountryFlag { get;  set; }
    }
}