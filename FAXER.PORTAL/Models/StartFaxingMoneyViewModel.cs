using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class StartFaxingMoneyViewModel
    {
        public const string BindProperty = "TopUpCard,TotalCardAmount,ReceivingCountryCode,ReceivingCurrency,ReceivingCurrencySymbol";
        [Required(ErrorMessage = "Enter Top Up Card")]
        [Display(Name = "Select Top Up Card")]
        public string TopUpCard { get; set; }

        [Display(Name = "Amount Of Money On Thiscard")]
        public double TotalCardAmount { get; set; }
        public string ReceivingCountryCode { get; internal set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
    }
    public enum PaymentOption
    {
        PaymentUsingCRDrCard,
        PaymentUsingSavedCRDrCard,
        BankToBank
    }
}