using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayAReceiverKiiPayWalletEnteramountViewModel
    {

        public const string BindProperty = "Amount , IsExpired ,LimitBalance,CashLimitType , WalletCurBalance ,WalletCurrencySymbol ,WalletName , WalletCurrencyCode ,AgentCommission";
        [Range(1, int.MaxValue, ErrorMessage = "Enter the amount")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Is Expired?")]

        public bool IsExpired { get; set; }
        public decimal LimitBalance { get; set; }
        public CardLimitType CashLimitType { get; set; }
        public decimal WalletCurBalance { get; set; }
        public string WalletCurrencySymbol { get; set; }
        public string WalletName { get; set; }
        public string WalletCurrencyCode { get; set; }
        public decimal AgentCommission { get; set; }
    }
}