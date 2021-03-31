using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalSelectCountryVM : KiiPayBusinessSearchSuppliersVM
    {
        [Required(ErrorMessage = "Please select transfer country to proceed")]
        public string Country { get; set; }
    }

}