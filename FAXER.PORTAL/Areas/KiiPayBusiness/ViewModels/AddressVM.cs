using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class AddressVM
    {
        public const string BindProperty = " Country,City ,AddressLine1 , AddressLine2, PostcodeORZipCode";

        [Required(ErrorMessage ="Please select the country")]
        public string Country { get; set; }


        [Required(ErrorMessage = "Please enter the city name")]
        public string City { get; set; }

        [Required(ErrorMessage ="Please select the City")]
        public string AddressLine1 { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string AddressLine2 { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string PostcodeORZipCode { get; set; }
    }
}