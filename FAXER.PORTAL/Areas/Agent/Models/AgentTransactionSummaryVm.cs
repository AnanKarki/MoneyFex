using FAXER.PORTAL.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentTransactionSummaryVm
    {
        public CashPickupInformationViewModel SenderDetails { get; set; }
        public RecipientsViewModel RecipientDetails { get; set; }
        public PaymentSummaryForAgentVm PaymentSummary { get; set; }
    }
    public class PaymentSummaryForAgentVm
    {
        public string SendingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal CommissionFee { get; set; }
        public bool IsManualDeposit { get; set; }
        public bool IsEuropeTransfer { get; set; }
    }
}