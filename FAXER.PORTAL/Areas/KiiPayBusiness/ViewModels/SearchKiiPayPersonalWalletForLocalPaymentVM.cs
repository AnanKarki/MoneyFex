using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class SearchKiiPayPersonalWalletForLocalPaymentVM
    {
        public const string BindProperty = "PhoneCode , MobileNo";
        public string PhoneCode { get; set; }
        [Required(ErrorMessage ="Please enter the phone no")]
        public string MobileNo { get; set; }

    }
}