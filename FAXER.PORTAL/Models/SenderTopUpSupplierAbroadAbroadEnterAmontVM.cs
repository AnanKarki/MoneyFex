using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTopUpSupplierAbroadAbroadEnterAmontVM
    {
        public const string BindProperty = "PhotoUrl,ReceiverName,ReceiverAccountNo,Fee,TotalAmount,ReceivingAmount,SendingAmount,SendingCurrency" +
            ",ReceivingCurrency,ExchangeRate,SendingCurrencySymbol,ReceivingCurrencySymbol,Amount,PaymentFrequencyId,SetStandingOrderPayment,FrequencyDetails";


        public string PhotoUrl { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAccountNo { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Fee { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal ReceivingAmount { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal SendingAmount { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Enter amount")]
  
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        [Range(0.0 , double.MaxValue)]
        public decimal ExchangeRate { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Amount { get; set; }
        public AutoPaymentFrequency PaymentFrequencyId { get; set; }
        public bool SetStandingOrderPayment { get; set; }
        public string FrequencyDetails { get; set; }
    }
}
