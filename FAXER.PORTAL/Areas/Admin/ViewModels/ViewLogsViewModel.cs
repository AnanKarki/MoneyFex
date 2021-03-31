using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewLogsViewModel
    {

        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public SystemLoginLevel LoginLevel { get; set; }
        public string LoginDate { get; set; }
        public string LoginTime { get; set; }
        public string LoginLocation { get; set; }
        public string LogoutDate { get; set; }
        public string LogoutTime { get; set; }
        public string LogoutLocation { get; set; }
        public string TDESystem { get; set; }
        public string CurrentLoginStatus { get; set; }
    }
}