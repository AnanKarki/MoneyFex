using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class MasterViewBusinessMerchantViewModel
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Date { get; set; }
        public string Heading { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string BusinessMerchant { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CountryFlag { get; set; }
    }
}