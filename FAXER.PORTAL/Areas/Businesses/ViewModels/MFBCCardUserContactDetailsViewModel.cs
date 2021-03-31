using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFBCCardUserContactDetailsViewModel
    {
        public const string BindProperty = "Address1 ,Address2 ,City , State, PostalCode ,CountryName ,Country ,PhoneNumber , EmailAddress";

        [Required]
        public string Address1 { get; set; }

        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string CountryName { get; set; }

        public string Country { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string EmailAddress { get; set; }


    }

    public class MFBCCardUserIdentification {

        public const string BindProperty = "CardPhotoURL";

        public string CardPhotoURL { get; set; }
    }
}