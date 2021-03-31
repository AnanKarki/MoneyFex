using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffLoginViewModel
    {
        public const string BindProperty = "StaffEmail , StaffPassword , StaffConfirmPassword, StaffCurrentLocation , LoginCode";


        [Required(ErrorMessage = "Enter Your Email")]
        public string StaffEmail { get; set; }

        [Required(ErrorMessage = "Enter Your Password")]
        public string StaffPassword { get; set; }
        public string StaffConfirmPassword { get; set; }
        //[Required(ErrorMessage = "Please Allow to Track the Location")]
        public string StaffCurrentLocation { get; set; }
        public string LoginCode { get; set; }
    }
    public class ConfirmPasswordResetViewModel
    {

        public const string BindProperty = "NewPassword , ConfirmPassword ";

        [Required, DisplayName("New Password")]
        public string NewPassword { get; set; }
        [Required, DisplayName("Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }

    public class LoggedStaff
    {

        public int StaffId { get; set; }

        public string FirstName { get; set; }
        public string Country { get; set; }

        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string LoginCode { get; set; }

        public string StaffMFSCode { get; set; }

        public string StaffCurrentLocation { get; set; }
        public SystemLoginLevel SystemLoginLevel { get; set; }

    }
}