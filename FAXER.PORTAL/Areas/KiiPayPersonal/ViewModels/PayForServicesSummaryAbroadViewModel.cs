using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayForServicesSummaryAbroadViewModel
    {
        public const string BindProperty = "Id ,Amount ,Fee , PayingAmount, ReceivingAmount, PaymentReference ,ReceiverName  ,SendersCurrencySymbol" +
          " ,SenderCurrencyCode ,ReceivingCurrencySymbol , ReceivingCurrencyCode  ";

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string ReceiverName { get; set; }
        public string SendersCurrencySymbol { get; set; }
        public string SenderCurrencyCode { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string ReceivingCurrencyCode { get; set; }
    }
}