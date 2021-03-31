using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class SendMoneyToKiiPayEnterAmountViewModel
    {
        public const string BindProperty = "WalletName , SendingAmount ,SendingCountry,SendingCurrency,ReceivingAmount , ReceivingCurrency ,SendingCurrencySymbol" +
            ",ReceivingCurrencySymbol , Currency ,Fee ,TotalAmount , TheyReceive ,IsConfirm," +
           "ExchangeRate , PaymentReference ,AgentCommission,ReceivingCountry";


        public string WalletName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal SendingAmount { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountry { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal ReceivingAmount { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string Currency { get; set; }
        public decimal Fee { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TheyReceive { get; set; }
        [Required(ErrorMessage = "Are you Confirm?")]
        public bool IsConfirm { get; set; }
        public decimal ExchangeRate { get; set; }
        [Required(ErrorMessage = "Enter Payment Reference")]
        public string PaymentReference { get; set; }
        public decimal AgentCommission { get; set; }
    }
}