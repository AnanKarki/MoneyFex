using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessCompleteLocalTransferVM
    {
        public string BusinessName { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string CurrencySymbol { get; set; }

    }

    public class KiiPayBusinessCompleteInternationalTransferVM
    {
        public string BusinessName { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string CurrencySymbol { get; set; }

    }
}