using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class FaxerContactDetails
    {
        public const string BindProperty = "Address1,Address2,City,State,Count,Country,PostalCode,PhoneNumber,CountryPhoneCode";

        [Required(ErrorMessage ="Enter Address")]
        [Display(Name = "Address1")]
        public string Address1 { get; set; }

        [Required(ErrorMessage ="Enter Address 2")]
        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Required(ErrorMessage ="Enter City")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Enter State/Province/Region")]
        [Display(Name = "State/Province/Region")]
        public string State { get; set; }

        [Required(ErrorMessage = "Select Country")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage ="Enter Zip/Code")]
        [Display(Name = "Zip/Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage ="Enter Phone Number")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone Number must be numeric")]
        public string PhoneNumber { get; set; }
        public string CountryPhoneCode { get; set; }
    }
}