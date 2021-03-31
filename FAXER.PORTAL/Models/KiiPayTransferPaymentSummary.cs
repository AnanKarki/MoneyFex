using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class KiiPayTransferPaymentSummary
    {


        public const string BindProperty = "ReceiverName,SendingCurrency,ReceivingCurrency,SendingCurrencySymbol," +
            "ReceivingCurrencySymbol,SendingAmount,ReceiverCity,ReceivingAmount,Fee," +
            "TotalAmount,ExchangeRate,PaymentReference, SendSMS ,SMSFee ";
        [MaxLength(200)]
        public string ReceiverName { get; set; }
        [MaxLength(200)]
        public string SendingCurrency { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrency { get; set; }
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }

        [Range(0.0, Double.MaxValue , ErrorMessage = "Enter amount")]
        
        public decimal SendingAmount { get; set; }
        public string ReceiverCity { get; set; }
        [Range(0.0, Double.MaxValue, ErrorMessage = "Enter amount")]

        public decimal ReceivingAmount { get; set; }
        
        [Range(0.0, Double.MaxValue)]

        public decimal Fee { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal TotalAmount { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal ExchangeRate { get; set; }
        [Required(ErrorMessage ="Enter the payment reference")]

        public string PaymentReference { get; set; }
        public bool SendSMS { get; set; }

        public decimal SMSFee { get; set; }
        public int TransactionSummaryId { get; set; } 




    }
}