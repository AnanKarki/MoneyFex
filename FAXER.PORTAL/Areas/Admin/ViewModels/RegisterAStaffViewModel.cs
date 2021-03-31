using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RegisterAStaffViewModel
    {
        public const string BindProperty = "Id , StaffFirstName ,StaffMiddleName ,StaffLastName , StaffDOB,StaffBirthCountry , StaffGender, StaffEmailAddress,ConfirmEmail  ";

        public int Id { get; set; }
        [Required]
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        [Required]
        public string StaffLastName { get; set; }
        [Required]
        public System.DateTime StaffDOB { get; set; }
        [Required]
        public string StaffBirthCountry { get; set; }
        public Gender? StaffGender { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string StaffEmailAddress { get; set; }
        public string ConfirmEmail { get; set; }

    }
}