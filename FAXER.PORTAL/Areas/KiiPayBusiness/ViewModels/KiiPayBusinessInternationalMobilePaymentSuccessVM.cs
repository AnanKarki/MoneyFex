using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalMobilePaymentSuccessVM
    {
        public int Id { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal Receiver { get; set; }
    }
}