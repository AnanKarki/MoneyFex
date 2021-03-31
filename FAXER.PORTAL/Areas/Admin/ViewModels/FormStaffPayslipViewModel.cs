using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class FormStaffPayslipViewModel
    {
        public const string BindProperty = "Id , Country ,City ,StaffId ,StaffMFSCode ,Year , Month,PayslipURL ";

        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int StaffId { get; set; }
        public string StaffMFSCode { get; set; }
        public int Year { get; set; }
        public Month Month { get; set; }
        public string PayslipURL { get; set; }
    }
}