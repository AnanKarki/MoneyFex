using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class CommonEnterAmountViewModel
    {
        public const string BindProperty = "SendingCurrencySymbol,ReceivingCurrencySymbol,SendingAmount" +
            ",ReceivingAmount,Fee,TotalAmount,ExchangeRate,SendingCountryCode,ReceivingCountryCode,SendingCurrency,ReceivingCurrency,AgentCommission";
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }

        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal SendingAmount { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal ReceivingAmount { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal Fee { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal ExchangeRate { get; set; }

        [MaxLength(200)]
        public string SendingCountryCode { get; set; }

        [MaxLength(200)]
        public string ReceivingCountryCode { get; set; }

        [MaxLength(200)]
        public string SendingCurrency { get; set; }

        [MaxLength(200)]
        public string ReceivingCurrency { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal AgentCommission { get; set; }

        public TransactionTransferMethod TransactionTransferMethod { get; set; }
    }
}