using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalTopUpVM
    {
    }
    public class KiiPayBusinessSearchCountryVM : KiiPayBusinessSearchSuppliersVM
    {
        public string Country { get; set; }
    }

    public class KiiPayBusinessInternationalTopUpEnterAccountNoVM
    {
        public const string BindProperty = " AccountNo ";

        public string AccountNo { get; set; }
    }

    public class KiiPayBusinessInternationalTopUpEnterAmountVM
    {
        public const string BindProperty = " AccountNo , SendingAmount , RecevingAmount, StandingOrderPaymentAmount,SendingCurrencySymbol , RecevingCurrencySymbol" +
            ", SendingCurrencyCode, RecevingCurrencyCode ,PaymentFrequency ,FrequencyDetails  ,StandingOrderPayment ,Fee , TotalAmount , ExchangeRate ";
        public string AccountNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount greater than {0}")]
        public decimal SendingAmount { get; set; }
        public decimal RecevingAmount { get; set; }
        public decimal StandingOrderPaymentAmount { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string RecevingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string RecevingCurrencyCode { get; set; }
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        public string FrequencyDetails { get; set; }
        public bool StandingOrderPayment { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate{ get; set; }
    }
    public class KiiPayBusinessInternationalTopUpSuccessVM {

        public const string BindProperty = " AccountNo , Amount ";
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
    }

    
}