using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class KiiPayPersonalStandingOrderPaymentVM
    {
    }
    public class KiiPayPersonalStandingOrderPaymentListVM
    {

        public int TransactionId { get; set; }

        /// <summary>
        /// Kii Pay personal Wallet Id or KiiPay Business Info Id (Receiver) 
        /// </summary>
        public int ReceiverId { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }

        public AutoPaymentFrequency PaymentFrequency { get; set; }

        public string FrequencyDetail { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class KiiPayPersonalBusinessStandingOrdervm
    {
        public const string BindProperty = " ReceiverId,ReceiverMobileNo , WalletId, Amount, PaymentFrequency , CurrencyCode, FrequencyDetials, CurrencySymbol, CountryCode ";
        public int ReceiverId { get; set; }
        public string ReceiverMobileNo { get; set; }
        public int WalletId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount")]
        public decimal Amount { get; set; }

        [Range(1, 3, ErrorMessage = "Please select the payment frequency")]
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = "Please select the frequency details ")]
        public string FrequencyDetials { get; set; }

        public string CurrencySymbol { get; set; }

        public string CountryCode { get; set; }
    }

    public class KiiPayPersonalAddNewBusinessStandingOrderSuccessvm
    {
        public const string BindProperty = " BusinessName , Amount, CurrencySymbol ";

        public string BusinessName { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
    }
    public class KiiPayPersonalUpdateBusinessStandingOrdervm
    {
        public const string BindProperty = " TransactionId,Amount , PaymentFrequency, FrequencyDetials, PreviousAmount , SenderCurrencySymbol,SenderCurrencyCode";
        public int TransactionId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount")]
        public decimal Amount { get; set; }
        [Range(1, 3, ErrorMessage = "Please select the payment frequency")]
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        [Required(ErrorMessage = "Please select the frequency details ")]
        public string FrequencyDetials { get; set; }

        public decimal PreviousAmount { get; set; }

        public string SenderCurrencySymbol { get; set; }
        public string SenderCurrencyCode { get; set; }

    }
}