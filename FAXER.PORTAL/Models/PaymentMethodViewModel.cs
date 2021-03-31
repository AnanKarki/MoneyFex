using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class PaymentMethodViewModel
    {

        public const string BindProperty = "PaymentMethod ,TotalAmount,SendingCurrencySymbol,SenderPaymentMode," +
            "EnableAutoPayment, AutopaymentFrequency,AutoPaymentAmount,PaymentDay" +
            "KiipayWalletBalance,HasKiiPayWallet,CardDetails";
        // Old 
        //[Required(ErrorMessage = "Please select Payment Method")]
        [Display(Name = "Payment Method")]
        [StringLength(200)]
        public string PaymentMethod { get; set; }

        [Range(0.0, Double.MaxValue)]

        // New Version  ViewModel property
        public decimal TotalAmount { get; set; }
        
        [StringLength(200)]
        public string SendingCurrencySymbol { get; set; }

        [Range(0,4)]
        public SenderPaymentMode SenderPaymentMode { get; set; }


        public bool EnableAutoPayment { get; set; }


        [Range(0, 4)]
        public AutoPaymentFrequency AutopaymentFrequency { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal AutoPaymentAmount { get; set; }

        [StringLength(200)]
        public string PaymentDay { get; set; }

        [Range(0.0, Double.MaxValue)]

        public decimal KiipayWalletBalance { get; set; }


        public bool HasKiiPayWallet { get; set; }
        public bool HasEnableMoneyFexBankAccount { get; set; }
        public int TransactionSummaryId { get; set; }

        public List<SenderSavedDebitCreditCard> CardDetails { get; set; }
    }

    
    public enum SenderPaymentMode
    {
        [Display(Name = "Credit/Debit Card")]
        [Description("Credit/Debit Card")]
        CreditDebitCard,
        [Display(Name = "Credit/Debit Card")]
        [Description("Credit/Debit Card")]
        SavedDebitCreditCard,
        [Display(Name = "KiiPay Wallet")]
        [Description("KiiPay Wallet")]
        KiiPayWallet,
        [Display(Name = "Bank Account")]
        [Description("Bank Account") ]
        MoneyFexBankAccount,
        Cash

    }
}