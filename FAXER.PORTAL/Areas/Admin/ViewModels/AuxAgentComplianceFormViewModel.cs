using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AuxAgentComplianceFormViewModel
    {
        public int Id { get; set; }
        public string AgentStaffId { get; set; }
        public string StaffId { get; set; }
        public string Country { get; set; }
        public string AgentName { get; set; }
        public string  SubDate{ get; set; }
        public string  FormAction{ get; set; }
        public string  ActionDate{ get; set; }

        public string Form { get; set; }
        public string CountryCode { get;  set; }
        public int FormId { get;  set; }
        public int AgentId { get;  set; }
    }
}