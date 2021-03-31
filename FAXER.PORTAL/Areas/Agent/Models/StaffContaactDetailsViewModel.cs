using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class StaffContaactDetailsViewModel
    {
        public const string BindProperty = "Id , Address1 ,Address2,City , State ,ZipCode,Country , PhoneCode ," +
         "PhoneNumber,EmailAddress , Password,ConfirmPassword";

        public int Id { get; set; }

        [Required(ErrorMessage ="Enter Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [Required(ErrorMessage ="Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage ="Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage ="Enter Zip Code"),DisplayName("Zip Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage ="Select Country")]
        public string Country { get; set; }

        public string PhoneCode { get; set; }


        [Required(ErrorMessage ="Enter Phone Number"), DisplayName("Mobile Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="Enter Email Address"), DisplayName("Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage ="Enter Password")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Enter Confirm Password"), DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}