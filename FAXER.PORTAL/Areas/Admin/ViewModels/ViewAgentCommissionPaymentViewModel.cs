using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewAgentCommissionPaymentViewModel
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentMFSCode { get; set; }
        public string Status { get; set; }
        public decimal TotalSentPayment { get; set; }
        public decimal SentCommissionRate { get; set; }
        public decimal TotalSentCommission { get; set; }
        public decimal TotalReceivedPayment { get; set; }
        public decimal ReceivedCommissionRate { get; set; }
        public decimal TotalReceivedCommission { get; set; }
        public decimal TotalCommission { get; set; }
        public bool IsVerified { get; set; }
        public string NameOfVerifier { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string AgentCurrencySymbol { get; set; }
        public virtual AgentInformation Agent { get; set; }
    }
}