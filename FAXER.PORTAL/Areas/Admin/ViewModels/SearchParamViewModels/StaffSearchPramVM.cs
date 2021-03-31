using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels
{
    public class StaffSearchPramVM
    {
        public string Country { get; set; }
        public string City { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string CreatedByName { get; set; }
        public string DateRange{ get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string StaffNoticeTitle { get; set; }
        public string Title { get; set; }
    }
}