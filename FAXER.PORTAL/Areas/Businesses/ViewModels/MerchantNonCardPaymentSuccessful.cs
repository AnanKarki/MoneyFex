using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MerchantNonCardPaymentSuccessful
    {

        public string MFCN { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverCountry { get; set; }
        public decimal SentAmount { get; set; }
        
        public decimal ReceivingAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        public string ReceiveOption { get; set; }

    }
}