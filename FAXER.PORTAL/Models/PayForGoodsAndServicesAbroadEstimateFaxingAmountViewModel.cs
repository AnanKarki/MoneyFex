using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class PayForGoodsAndServicesAbroadEstimateFaxingAmountViewModel
    {
        public const string BindProperty = "FaxingAmount,ReceivingAmount,IncludeFaxingFee,PaymentReference" +
            ", FaxingCurrency, FaxingCurrencySymbol, ReceivingCurrency, ReceivingCurrencySymbol";


        [Required(ErrorMessage = "Enter Paying Amount")]
        [Display(Name = "Paying Amount")]
        public decimal FaxingAmount { get; set; }

        [Required(ErrorMessage = "Enter Receiving Amount")]
        [Display(Name = "Amount to be Received")]
        public decimal ReceivingAmount { get; set; }

        [Display(Name = "Does Faxing Amount Include Faxing Fee ?")]
        public bool IncludeFaxingFee { get; set; }

        [Required(ErrorMessage = "Enter Paying Reference")]
        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
    }
}