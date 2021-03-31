using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAccountPaymentSummaryViewModel
    {

        public const string BindProperty = "Id,SendingCurrencySymbol, SendingCurrencyCode , Amount ,Fee  , LocalSmsCharge, PaidAmount," +
            "ReceivedAmount,PaymentReference, ReceivingCurrencySymbol , ReceivingCurrecyCoode , ReceiverName";

        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string SendingCurrencyCode { get; set; }
        [Range(0.0, Double.MaxValue)]


        public decimal Amount { get; set; }
        [Range(0.0, Double.MaxValue)]


        public decimal Fee { get; set; }
        [Range(0.0, Double.MaxValue)]


        public decimal LocalSmsCharge { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal PaidAmount { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal ReceivedAmount { get; set; }

        [MaxLength(200)]
        public string PaymentReference { get; set; }

        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }

        [MaxLength(200)]
        public string ReceivingCurrecyCoode { get; set; }

        [MaxLength(200)]
        public string ReceiverName { get; set; }


        
                

    }
}