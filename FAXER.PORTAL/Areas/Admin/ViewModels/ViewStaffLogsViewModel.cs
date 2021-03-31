using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewStaffLogsViewModel
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        public string StaffLastName { get; set; }
        public string StaffCountry { get; set; }
        public string StaffCity { get; set; }
        public SystemLoginLevel StaffLoginLevel { get; set; }
        public string LoginDate { get; set; }
        public string LoginTime { get; set; }
        public string LoginLocation { get; set; }
        public string LogoutDate { get; set; }
        public string LogoutTime { get; set; }
        public string LogoutLocation { get; set; }
        public string TimeLoginToSystem { get; set; }
        public string CurrentLoginStatus { get; set; }

        
    }
}