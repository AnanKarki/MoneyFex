using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAutoPaymentAddViewModel
    {

        public const string BindProperty = "Id,Amount,PaymentFrequency,SendingCurrencySymbol,Currency,MobileNo,FrequencyDetails,CurrentOrder,Availablebalance";
        public int Id { get; set; }
        [Range(0.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal Amount { get; set; }
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string Currency { get; set; }
        public string MobileNo { get; set; }
        public string FrequencyDetails { get; set; }
        public string CurrentOrder { get; set; }
        public decimal Availablebalance { get; set; }


    }
}