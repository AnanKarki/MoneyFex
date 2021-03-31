using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessSearchSuppliersVM
    {
        public const string BindProperty = " SupplierId, WalletNo , Supplier";
        [Required(ErrorMessage = "Please enter the Wallet No")]
        public int SupplierId { get; set; }
        public string WalletNo { get; set; }

        public string Supplier { get; set; }
    }
    public class SuppliersDropDownVM {
        public string WalletNo { get; set; }
        public string SuppliersName { get; set; }
    }
    public class PaymentFrequencyDropDownVM
    {
        public string FrequencyName { get; set; }
    }
    public class KiiPayBusinessLocalTopUpSuccessVM
    {
        public const string BindProperty = " AccountNo , Amount";

        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
    }

    
}