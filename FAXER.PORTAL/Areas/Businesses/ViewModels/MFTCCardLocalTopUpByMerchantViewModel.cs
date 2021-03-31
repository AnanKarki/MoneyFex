using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFTCCardLocalTopUpByMerchantViewModel
    {
        public const string BindProperty = "FaxingAmount ,PaymentReference ";
        
        public decimal FaxingAmount { get; set; }
        public string PaymentReference { get; set; }
    }
}