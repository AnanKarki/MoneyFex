using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MerchantInternationPaymentSuccessfulByCarduserViewModel
    {

        public string MerchantName { get; set; }

        public string MerchantAccountNo { get; set; }

        public string Country { get; set; }

        public string TotalAmount { get; set; }

        public string PaymentReference { get; set; }

        public string ReceiveOption { get; set; }
    }
}