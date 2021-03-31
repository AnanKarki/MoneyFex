using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class LoggedUserVm
    {
        public int Id { get; set; }
        public int PayingAgentStaffId { get; set; }
        public string PayingAgentStaffName { get; set; }
        public string PayingAgentAccountNumber { get; set; }
        public StaffType AgentType { get; set; }
        public bool IsAUXAgent { get; set; }

    }
}