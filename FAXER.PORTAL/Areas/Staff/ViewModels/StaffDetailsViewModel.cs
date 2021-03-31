using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffDetailsViewModel
    {
        public const string BindProperty = "StaffFirstName , StaffMiddleName , StaffLastName , StaffDateOfBirth , StaffBirthCountry , StaffGender , StaffEmailAddress , CofirmStaffEmailAddress " ;
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        
        public string StaffLastName { get; set; }
        
        public System.DateTime StaffDateOfBirth { get; set; }
        
        public string StaffBirthCountry { get; set; }

        public Gender StaffGender { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string StaffEmailAddress { get; set; }
        
        public string CofirmStaffEmailAddress { get; set; }
    }
}