using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class PaymentUsingSavedCreditDebitCardVm
    {

        public const string BindProperty = "TopUpAmount,SavedCard ,NameOnCard,CardNumberMasked,CardNumber,EndMonth,EndYear,SecurityCode,Address1,Address2,City" +
            ",Country,AutoTopUp ,AutoTopUpAmount,AutoPaymentFrequency,PaymentDay,Confirm,FaxingCurrency,FaxingCurrencySymbol,StripeTokenID";


        public decimal TopUpAmount { get; set; }
        public string SavedCard { get; set; }
        [Required(ErrorMessage = "Enter Name on Card")]
        public string NameOnCard { get; set; }
        public string CardNumberMasked { get; set; }
        [Required(ErrorMessage = "Enter Card Number Masked")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Enter Card Number")]
        public string EndMonth { get; set; }
        
        [Required(ErrorMessage = "Enter End Month")]
        public string EndYear { get; set; }

        [Required(ErrorMessage = "Enter Security Code")]
        public string SecurityCode { get; set; }

        [Required(ErrorMessage = "Enter Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Enter Postal Code")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Enter Country")]
        public string Country { get; set; }
        public bool AutoTopUp { get; set; }
        public decimal AutoTopUpAmount { get; set; }
        public DB.AutoPaymentFrequency AutoPaymentFrequency { get; set; }
        
        public string PaymentDay { get; set; }
        public bool Confirm { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string StripeTokenID { get; internal set; }
    }
}