using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessLocalTopUpEnterAccountNoVM
    {
        public const string BindProperty = " AccountNo";

        [Required(ErrorMessage = "Please enter the AccountNo to proceed")]
        public string AccountNo { get; set; }
    }

    public class KiiPayBusinessLocalTopUpEnterAmountVM
    {

        public const string BindProperty = " AccountNo , Amount, StandingOrderPaymentAmount,CurrencySymbol , CurrencyCode ,FrequencyDetials , PaymentFrequency , StandingOrderPayment";
        public string AccountNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount greater than {0}")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "Please select the frequency details ")]
        public decimal StandingOrderPaymentAmount { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public string FrequencyDetials { get; set; }
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        public bool StandingOrderPayment { get; set; }
    }
}