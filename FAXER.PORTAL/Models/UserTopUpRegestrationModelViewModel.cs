using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class UserTopUpRegestrationModelViewModel
    {
        [Required(ErrorMessage ="Enter Address")]
        [Display(Name = "Address1")]
        public string Address1 { get; set; }


        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Required(ErrorMessage ="Enter State/Province/Region")]
        [Display(Name = "State/Province/Region")]
        public string State { get; set; }


        [Required(ErrorMessage ="Enter Zip/Postal Code")]
        [Display(Name = "Zip/Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage ="Enter City")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage ="Select Countty")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage ="Enter Phone Number")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone Number must be numeric")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage ="Enter Email Address")]
        [Required(ErrorMessage = "Enter Email Addresss")]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }


        [Required(ErrorMessage = "Enter First Name")]
        [Display(Name = "First Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string TopUpUserFirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string TopUpUserMiddleName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"[a-z A-Z]+$", ErrorMessage = "Use letters only please.")]
        public string TopUpUserLastName { get; set; }

        [Required(ErrorMessage = "Enter Date of Birth")]
        [Display(Name = "Date of Birth")]
        public DateTime TopUpUserDateOfBirth { get; set; }

        public string ImageURL { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }



    }
}