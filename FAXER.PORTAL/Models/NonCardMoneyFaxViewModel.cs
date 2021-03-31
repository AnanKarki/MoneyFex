using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class NonCardMoneyFaxViewModel
    {

        public const string BindProperty = "ReceivingCountry,FaxingAmount,ReceivingAmount," +
            "IncludeFaxingFee,SendingCurrency,SendingCurrencySymbol,ReceivingCurrency,ReceivingCurrencySymbol";
        [Required(ErrorMessage ="Select Receiving Country")]       
        public string ReceivingCountry { get; set; }

        [Display(Name = "Faxing Amount")]
        [Range(0.0, Double.MaxValue)]

        public decimal FaxingAmount { get; set; }

        [Display(Name = "Amount To Be Received")]
        [Range(0.0, Double.MaxValue)]

        public decimal ReceivingAmount { get; set; }
        [Display(Name = "Does the Transfer Amount Include Fee ?")]
        public bool IncludeFaxingFee { get; set; }
        public string SendingCurrency { get; set; }
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrency { get; set; }
        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }
    }
}