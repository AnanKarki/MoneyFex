using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class PaymentCompletedVM
    {

        public string ReceiverName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }

    }
}