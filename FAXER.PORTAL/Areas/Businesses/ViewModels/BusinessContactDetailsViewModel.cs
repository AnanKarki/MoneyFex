using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class BusinessContactDetailsViewModel
    {
        public const string BindProperty = "Address1, Address2,City ,State ,PostalCode , Country, PhoneNumber,FaxNo , Website, IsConfirmed";
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
        public string Country { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FaxNo { get; set; }

        public string Website { get; set; }
        public bool IsConfirmed { get; set; }

    }
}