using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class MobileMoneyTransferSuccessViewModel
    {
        public string SendingCurrency { get; set; }
        public decimal SendingAmount { get; set; }
        public string WalletName { get; set; }
        public int TransactionId { get; set; }
    }
}