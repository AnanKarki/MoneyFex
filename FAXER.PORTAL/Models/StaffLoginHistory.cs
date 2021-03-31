using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class StaffLoginHistory
    {
        public int Id { get; set; }
        public int staffId { get; set; }
        public DateTime LoginDate { get; set; }
        public TimeSpan LoginTime { get; set; }
        public string LoginLocation { get; set; }
        public DateTime LogoutDate { get; set; }
        public TimeSpan LogoutTime { get; set; }
        public string LogoutLocation { get; set; }
        public string TimeLoginToSystem { get; set; }
        public bool CurrentLoginStatus { get; set; }
    }
}