using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class ComplianceCommission
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AgentStaffId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public bool Status { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ModifierStaffType ModifierStaffType { get; set; }
        public virtual AgentStaffInformation AgentStaff { get; set; }
    }

    public enum ModifierStaffType
    {
        
        AgentStaff = 1,
        AdminStaff = 2
    }
}