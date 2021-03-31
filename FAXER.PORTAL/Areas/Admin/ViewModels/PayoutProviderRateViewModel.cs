using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PayoutProviderRateViewModel
    {
        public const string BindProperty = "Id , SendingCurrency, RecevingCurrency, SendingCountry,SendingCountryName , RecevingCountry," +
            " RecevingCountryName, PayoutProvider,PayoutProviderName ,Rate";
        public int Id { get; set; }
        [Required(ErrorMessage = "select Sending Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "select Receiving Currency")]
        public string RecevingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }
        public string RecevingCountry { get; set; }
        public string RecevingCountryName { get; set; }
        public Apiservice PayoutProvider { get; set; }
        public string PayoutProviderName { get; set; }
        [Required(ErrorMessage = "Enter Rate")]
        public decimal Rate { get; set; }
    }
}