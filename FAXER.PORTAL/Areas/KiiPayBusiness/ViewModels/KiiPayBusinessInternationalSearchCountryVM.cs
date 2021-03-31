using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalSearchCountryVM
    {
        public const string BindProperty = "Country";
        [Required(ErrorMessage = "Please select the Country")]
        public string Country { get; set; }
    }
}