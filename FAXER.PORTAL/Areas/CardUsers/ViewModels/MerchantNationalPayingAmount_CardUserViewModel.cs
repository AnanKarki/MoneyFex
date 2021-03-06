using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MerchantNationalPayingAmount_CardUserViewModel
    {
        public const string BindProperty = "FaxingAmount ,PaymentReference";

        public decimal FaxingAmount { get; set; }
        public string PaymentReference { get; set; }
    }

    public class MerchantInternationalPayingAmount_CardUserViewModel
    {
        public const string BindProperty = "FaxingAmount ,ReceivingAmount ,IncludingFee ,PaymentReference";
        public decimal FaxingAmount { get; set; }

        public decimal ReceivingAmount { get; set; }
        public bool IncludingFee { get; set; }

        public string PaymentReference { get; set; }
    }
}