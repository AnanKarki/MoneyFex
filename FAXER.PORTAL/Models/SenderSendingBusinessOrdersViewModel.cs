using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Models
{
    public class SenderSendingBusinessOrdersViewModel
    {
        public string MobileNo { get; set; }

        public string WalletId{ get; set; }
        public int  Id{ get; set; }
        public string Country { get; set; }
        public string WalletName { get; set; }
        public string City { get; set; }
        public string AutoAmount { get; set; }
        public string FrequencyDetails { get; set; }
        public string PaymentFrequency { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
        public string EnableAutoPayment { get; set; }


    }
    public class MobileNumberDropDown
    {
        public int Id { get; set; }
        public string MobileNo { get; set; }
    }
}