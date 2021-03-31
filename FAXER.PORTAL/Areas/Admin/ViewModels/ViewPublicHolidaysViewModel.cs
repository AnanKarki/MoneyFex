using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewPublicHolidaysViewModel
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        public string HolidayName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}