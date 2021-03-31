using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewStaffPayslipViewModel
    {
        public const string BindProperty = "Id,StaffId,StaffName,StaffMFSCode,StaffCountry,StaffCity,PayslipMonth,Month,PayslipYear,StaffStatus,PayslipPDF,StaffCountryFlag";
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string StaffMFSCode { get; set; }
        public string StaffCountry { get; set; }
        public string StaffCity { get; set; }
        public string PayslipMonth { get; set; }
        public Month Month { get; set; }
        public int PayslipYear { get; set; }
        public string StaffStatus { get; set; }
     
        public string PayslipPDF { get; set; }
      
        public string StaffCountryFlag { get;  set; }
        public string DueDate { get;  set; }
    }
}