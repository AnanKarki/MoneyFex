using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminRegisteredAgentStaffViewModel
    {
        public const string BindProperty = " Id ,BranchName ,StaffName , TelePhoneNumber,EmailAddress , Password, Address ,AccountNumber ";

        public int Id { get; set; }
        public string BranchName { get; set; }
        [Required(ErrorMessage = "Enter Staff Name")]
        public string StaffName { get; set; }
        [Required(ErrorMessage = "Enter TelePhone Number ")]
        public string TelePhoneNumber { get; set; }
        [Required(ErrorMessage = "Enter Email Address ")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string Address { get; set; }
        public string AccountNumber { get; set; }


    }
}