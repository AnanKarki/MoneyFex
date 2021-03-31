using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class AccountPaymentSummaryAbroadViewModel
    {
        public const string BindProperty = "Id ,AvailableBalance ,Amount ,Fee , PayingAmount ,ReceivingAmount ,PaymentReference ,SendingCurrency " +
            ",ReceivingCurrency ,SendingCurrencySymbol ,ReceivingCurrencySymbol , ReceiversName";
        public int Id { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string ReceiversName { get; set; }
    }
}