using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderWithdrawMoneyFromWalletSuccessViewModel
    {
        public decimal Amount { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
    }
}