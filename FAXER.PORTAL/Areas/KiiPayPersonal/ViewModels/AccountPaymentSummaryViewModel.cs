using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class AccountPaymentSummaryViewModel
    {
        public const string BindProperty = "Id ,SendingCurrencySymbol ,ReceivingCurrencySymbol ,SendingCurrencyCode , ReceivingCurrencyCode , AvailableBalance" +
            ", Amount , Fee ,ExchangeRate ,LocalSMSMessage ,PayingAmount ,ReceivingAmount ,PaymentReference , ReceiversName ";
        public int Id { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal  ExchangeRate { get; set; }
        public decimal LocalSMSMessage { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string ReceiversName { get; set; }
    }
}