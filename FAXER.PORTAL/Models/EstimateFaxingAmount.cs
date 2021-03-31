using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class EstimateFaxingAmount
    {
        public const string BindProperty = "FaxingAmount,ReceivingAmount,IncludeFaxingFee,FaxingCurrency,FaxingCurrencySymbol,ReceivingCurrency,ReceivingCurrencySymbol";

        [Display(Name = "Faxing Amount")]
        public decimal FaxingAmount { get; set; }

        [Display(Name = "Amount To Be Received")]
        public decimal ReceivingAmount { get; set; }

        [Display(Name ="Does Faxing Amount Include Faxing Fee ?")]
        public bool IncludeFaxingFee { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
    }
}