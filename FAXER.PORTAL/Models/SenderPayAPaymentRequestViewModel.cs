using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderPayAPaymentRequestViewModel
    {

        public const string BindProperty = "Id,PhotoUrl,ReceiverName," +
            "ReceiverWalletId,Amount,Fee,PayingAmount,ReceivingAmount," +
            "ExchangeRate,ReceiverCurrencySymbol,SendingCurrencySymbol,AvailableBalance";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string PhotoUrl { get; set; }
        public string ReceiverName { get; set; }
        [Range(0, int.MaxValue)]
        public int ReceiverWalletId { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Amount { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Fee { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal PayingAmount { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal ReceivingAmount { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal ExchangeRate { get; set; }
        [MaxLength(200)]
        public string ReceiverCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal AvailableBalance { get; set; }
    }
}