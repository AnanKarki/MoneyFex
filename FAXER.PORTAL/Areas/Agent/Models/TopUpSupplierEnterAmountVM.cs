using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class TopUpSupplierEnterAmountVM
    {
        public const string BindProperty = "PhotoUrl , ReceiverName ,ReceiverAccountNo,Fee , TotalAmount ,SendingAmount,ReceivingAmount , SendingCurrency ," +
            "ReceivingCurrency,ExchangeRate , SendingCurrencySymbol,ReceivingCurrencySymbol,AccountNo , AgentCommission";

        public string PhotoUrl { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAccountNo{ get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        [Range(1  , int.MaxValue  , ErrorMessage = "Enter amount") ]
        public decimal SendingAmount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal ReceivingAmount { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string AccountNo{ get; set;}
        public decimal AgentCommission { get; set; }

    }
}