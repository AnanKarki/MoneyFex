using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessSearchBusinessProviderVM
    {
        public const string BindProperty = " MobileNo ,RecentPaidBusiness ";
        [Required (ErrorMessage ="Please enter the mobile no")]
        [StringLength(10 , ErrorMessage ="Please enter the valid mobile no")]
        public string MobileNo { get; set; }

        public string RecentPaidBusiness { get; set; }


    }

    public class KiiPayBusinessInternationalSearchBusinessProviderVM  : KiiPayBusinessSearchBusinessProviderVM
    {
        [Required(ErrorMessage = "Please select transfer country to proceed")]
        public string Country { get; set; }
    }

    public class RecentPaidBusinessesDropDownVM {

        public string MobileNo { get; set; }
        public string BusinessName { get; set; }


    }
}