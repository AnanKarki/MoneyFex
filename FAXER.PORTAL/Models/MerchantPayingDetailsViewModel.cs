using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MerchantPayingDetailsViewModel
    {
        public const string BindProperty = "PayingAmount,PaymentFee,TotalAmountIncludingFee,CurrentExchangeRate,AmountToBeReceivedByMerchant," +
            "PaymentReference,FaxingCurrency,FaxingCurrencySymbol," +
            "ReceivingCurrency,ReceivingCurrencySymbol";

        [Range(0.0, Double.MaxValue)]

        public decimal PayingAmount { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal PaymentFee { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal TotalAmountIncludingFee { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal CurrentExchangeRate { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal AmountToBeReceivedByMerchant { get; set; }
        [MaxLength(200)]
        public string PaymentReference { get; set; }
        [MaxLength(200)]
        public string FaxingCurrency { get; set; }
        [MaxLength(200)]
        public string FaxingCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrency { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }
    }
}