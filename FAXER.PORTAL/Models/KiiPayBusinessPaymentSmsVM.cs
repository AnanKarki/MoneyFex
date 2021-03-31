using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class KiiPayBusinessPaymentSmsVM
    {

        public string SenderName { get; set; }
        public string ReceiverBusinessMobileNo { get; set; }
        public string ReceiverBusinessName { get; set; }
        public string SendingAmount { get; set; }

        public string ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string SenderCountry { get; internal set; }
        public string ReceiverCountry { get; internal set; }
        public string SenderPhoneNo { get; internal set; }
    }
}