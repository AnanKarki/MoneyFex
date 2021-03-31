using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderLocalEnterAmountVM
    {

        public const string BindProperty = "Id,Amount,CurrencySymbol,CurrencyCode," +
            "PaymentReference,ReceiverName,ReceiverId,ReceiverImage,SendSms";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }

        [Range(1.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal Amount { get; set; }
        [MaxLength(200)]
        public string CurrencySymbol { get; set; }
        [MaxLength(200)]
        public string CurrencyCode { get; set; }
        [MaxLength(200)]
        public string PaymentReference { get; set; }
        [MaxLength(200)]
        public string ReceiverName { get; set; }
        [Range(0 , int.MaxValue)]
        public int ReceiverId { get; set; }
        [MaxLength(200)]
        public string ReceiverImage { get; set; }
        public bool SendSms { get; set; }
    }
}