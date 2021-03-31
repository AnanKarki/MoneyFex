using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class NonCardFaxingFeeSummaryViewModel
    {
        public const string BindProperty = "FaxingAmount,FaxingFee,TotalAmount,ExchangeRate,ReceivingAmount";



        [Display(Name = "Faxing Amount")]
        public double FaxingAmount { get; set; }

        [Display(Name = "Faxing Fee")]
        public double FaxingFee { get; set; }

        [Display(Name = "Total Amount Including Fee")]
        public double TotalAmount { get; set; }

        [Display(Name = "Current Exchange Rate")]
        public double ExchangeRate { get; set; }

        [Display(Name = "Amount To Be Received")]
        public double ReceivingAmount { get; set; }
    }
}