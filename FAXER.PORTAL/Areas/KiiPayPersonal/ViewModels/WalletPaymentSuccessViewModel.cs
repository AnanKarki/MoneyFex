using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class WalletPaymentSuccessViewModel
    {
        public int Id { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal SentAmount { get; set; }
        public string Receiver { get; set; }
    }
}