using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class KiiPayPersonalPaymentSMSVM
    {

        public string SenderName { get; set; }
        public string SendingAmount { get; set; }
        public string ReceivingAmount { get; set; }
        public string SenderCountry { get; set; }
        public string ReceiverCountry { get; set; }

        public string SenderPhoneNo { get; set; }
        public string ReceiverPhoneNo { get; set; }

    }
}