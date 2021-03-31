using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewAgentStaffsViewModel
    {
        public int Id { get; set; }
        public int AgentStaffId { get; set; }
        public string StaffName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string StaffCode { get; set; }
        public bool Status { get; set; }
        
    }
}