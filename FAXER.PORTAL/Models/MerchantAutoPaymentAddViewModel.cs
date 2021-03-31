using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MerchantAutoPaymentAddViewModel
    {
        public const string BindProperty = "AutoPaymentAmount,AutoPaymentFrequency,PaymentReference,FrequencyDetails";
        public decimal AutoPaymentAmount { get; set; }
        public AutoPaymentFrequency AutoPaymentFrequency { get; set; }

        public string PaymentReference { get; set; }


        public string FrequencyDetails { get; set; }
    }


    public class SomeoneElseAutoPaymentViewModel {

        public const string BindProperty = "MFTCCardid,AutoPaymentAmount,AutoPaymentFrequency,FrequencyDetails,TopUpReference";
        public int MFTCCardid { get; set; }
        
        public decimal AutoPaymentAmount { get; set; }
        public AutoPaymentFrequency AutoPaymentFrequency { get; set; }

        public string FrequencyDetails { get; set; }

        public string TopUpReference { get; set; }

    }
}