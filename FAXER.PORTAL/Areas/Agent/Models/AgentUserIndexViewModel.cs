using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentUserIndexViewModel
    {
        public int Id { get; set; }
        public string StaffName{ get; set; }
        public StaffType StaffType{ get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek StartDay { get; set; }
        public DayOfWeek EndDay { get; set; }

        public bool Status { get; set; }
        public string Action { get; set; }
    }

    public enum StaffType
    {
        Admin,
        Transaction
    }
}