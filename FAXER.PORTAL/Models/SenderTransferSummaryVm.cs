using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTransferSummaryVm
    {

        public const string BindProperty = "Id ,SendingCurrencyCode, SendingCurrencySymbol , ReceivingCurrencyCode , ReceivingCurrencySymbol" +
            "Amount,Fee,PaidAmount ,ReceiverName ,ReceivedAmount, LocalSmsCharge";

        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [StringLength(200)]
        public string SendingCurrencyCode { get; set; }
        [StringLength(200)]
        public string SendingCurrencySymbol { get; set; }
        [StringLength(200)]
        public string ReceivingCurrencyCode { get; set; }
        [StringLength(200)]
        public string ReceivingCurrencySymbol { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Amount { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Fee { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal PaidAmount { get; set; }
        [StringLength(200)]
        public string ReceiverName { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal ReceivedAmount { get; set; }
        [StringLength(200)]
        public string PaymentReference { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal LocalSmsCharge { get; set; }
    }
}