using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderWithdrawMoneyFromWalletEnterAmountViewModel
    {
        public const string BindProperty = "AvailableCurrency,AvailableBalance,Amount,SendingCurrency,SendingCurrencySymbol";

        public string AvailableCurrency{ get; set; }
        public decimal AvailableBalance { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Enter amount")]
        
        public decimal Amount { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }

    }
}