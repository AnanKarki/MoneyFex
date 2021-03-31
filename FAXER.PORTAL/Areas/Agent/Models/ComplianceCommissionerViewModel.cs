using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{

    public class complianceCommissionVm
    {
        public List<ComplianceCommissionerViewModel> complianceCommissionsList { get; set; }
        public int FilterStaffId { get; set; }
        
        public int FilterDay { get; set; }
        public Month FilterMonth { get; set; }
        public int FilterYear { get; set; }

    }
    public class ComplianceCommissionerViewModel
    {
        public int Id { get; set; }
        public int AgentStaffId { get; set; }
        public string StaffName { get; set; }
        public string StaffType { get; set; }
        public string AppointmentDate { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
        public DateTime AppointmentDateTime { get; set; }


    }

    public class AgentStaffListViewModel
    {
        public int Id { get; set; }
        public string AgentStaffName { get; set; }
    }
}