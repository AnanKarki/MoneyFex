using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class EstimateFaxingFeeSummary
    {
        public const string BindProperty = "FaxingAmount,FaxingFee,TotalAmount,ExchangeRate,ReceivingAmount," +
            "IncludingFaxingFee,FaxingCurrency,FaxingCurrencySymbol,ReceivingCurrency,ReceivingCurrencySymbol";
        [Display(Name = "Faxing Amount")]
        [Range(0.0, Double.MaxValue)]
        public decimal FaxingAmount { get; set; }

        [Display(Name = "Faxing Fee")]
        [Range(0.0, Double.MaxValue)]
        public decimal FaxingFee { get; set; }


        [Display(Name = "Total Amount Including Fee")]
        [Range(0.0, Double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Current Exchange Rate")]
        [Range(0.0, Double.MaxValue)]
        public decimal ExchangeRate { get; set; }

        [Display(Name = "Amount To Be Received")]
        [Range(0.0, Double.MaxValue)]
        public decimal ReceivingAmount { get; set; }
        
        public bool IncludingFaxingFee { get; set; }
        [MaxLength(200)]
        public string FaxingCurrency { get; set; }
        [MaxLength(200)]
        public string FaxingCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrency { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }

        public bool IsIntroductoryRate { get; set; }
        public bool IsIntroductoryFee { get; set; }
        public decimal ActualFee { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }

    }
}