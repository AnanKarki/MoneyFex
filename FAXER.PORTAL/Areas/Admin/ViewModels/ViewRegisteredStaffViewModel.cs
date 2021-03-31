using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredStaffViewModel
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        public string StaffLastName { get; set; }
        public string StaffDOB { get; set; }
        public Gender StaffGender { get; set; }
        public string PrivateEmail { get; set; }
        public string MFSEmail { get; set; }
        public string StaffAddress1 { get; set; }
        public string StaffCity { get; set; }
        public string StaffCountry { get; set; }
        public string StaffTelephone { get; set; }
        public string TimeAtCurrentAddress { get; set; }
        public string StaffMFSCode { get; set; }
        public string Status { get; set; }
        public string FirstLetterOfStaff { get; set; }

    }
}