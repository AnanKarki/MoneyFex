using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffLoginHistory
    {
        public int Id { get; set; }
        [ForeignKey("StaffInformation")]
        public int StaffInformationId { get; set; }
        public DateTime LoginDate { get; set; }
        public TimeSpan LoginTime { get; set; }
        public string LoginLocation { get; set; }
        public DateTime? LogoutDate { get; set; }
        public TimeSpan? LogoutTime { get; set; }
        public string LogoutLocation { get; set; }
        public TimeSpan TimeLoginToSystem { get; set; }
        public bool CurrentLoginStatus { get; set; }
        public bool IsDeleted { get; set; }
        public virtual StaffInformation StaffInformation { get; set; }
    }
}