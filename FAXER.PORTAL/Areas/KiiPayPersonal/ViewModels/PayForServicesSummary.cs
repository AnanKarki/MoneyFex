using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayForServicesSummaryViewModel
    {
        public const string BindProperty = "Id ,Amount ,Fee , LocalSMSMessage, PayingAmout, ReceivingAmount ,PaymentReference  ,SendingCurrencySymbol" +
            " ,SendingCurrencyCode ,ReceivingCurrencySymbol , ReceivingCurrencyCode ,ExchangeRate  ";
        public int Id { get; set; }
        public decimal   Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal LocalSMSMessage { get; set; }
        public decimal PayingAmout { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}