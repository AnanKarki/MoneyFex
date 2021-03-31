using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessPaymentCompletedVM
    {

        public string ReceiverName { get; set; }

        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal CurrentBalance { get; set; }

        public string ReceivingCurrencySymbol { get; set; }
        public string SendingCurrencySymbol { get; set; }


    }
}