using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewAgentStaffMoreDetailsViewModel
    {
        public int Id { get; set; }
        public string AgentStaffLevel { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string IssuingCountry { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDay { get; set; }
        public string EndDay { get; set; }
        public string StaffName { get; set; }
        public string StaffIDUrl { get; set; }
        public string PassportSide1Url { get; set; }
        public string PassportSide2Url { get; set; }
    }
}