using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class WalletUsageControlViewModel
    {
        public int Id { get; set; }
        public string WithdrawalAmount { get; set; }
        public string WithdrawalType { get; set; }
        public string PurchaseAmount { get; set; }
        public string PurchaseType { get; set; }
    }
}