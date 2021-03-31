using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PersonalAddressViewModel
    {
        public const string BindProperty = "Id ,CountryCode ,City , Address1 , Address2 ,PostalCode";

        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
    }
}