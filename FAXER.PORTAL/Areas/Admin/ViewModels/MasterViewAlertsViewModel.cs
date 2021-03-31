using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class MasterViewAlertsViewModel
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Heading { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        //Treated sender,staff agent name as same
        public string Agent { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string FullMessage { get; set; }
        public string CountryFlag { get; set; }
    }
}