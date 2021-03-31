using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MerchantInternationalPaymentSuccessfulViewModel
    {
        public string MerchantAccountNo { get; set; }

        public string MerchantName { get; set; }

        public string MerchantCountry { get; set; }

        public decimal FaxingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ExchangeRate { get; set; }


        public string PaymentReference { get; set; }

        public string ReceiveOption { get; set; }

    }
}