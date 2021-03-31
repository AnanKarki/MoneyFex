using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_MerchantInternationalPaymentSuccessful
    {
        public string MerchantName { get; set; }
        public string MerchantAccountNo { get; set; }
        public string MerchantCountry { get; set; }
        public string AmountPaid { get; set; }
        public string PaymentReference { get; set; }
        public string ReceiveOption { get; set; }

    }
}