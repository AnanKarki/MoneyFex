using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class AdminPortalStaffLoginViewModel
    {
        public const string BindProperty = "StaffId , StaffNameAndSurname , StaffMFSCode, IsAuthorizedChecked , TimeZone";

        public int StaffId { get; set; }
        public string StaffNameAndSurname { get; set; }
        public string StaffMFSCode { get; set; }
        public bool IsAuthorizedChecked { get; set; }
        public string TimeZone { get; set; }
    }
}