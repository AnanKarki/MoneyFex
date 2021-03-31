using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AUXAgentRecentTransactionViewModel
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountrycode { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string AgentName { get; set; }
        public int AgentId { get; set; }
        public int SenderId { get; set; }
        public int RecipentId { get; set; }
        public string AgentAccountNo { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public decimal Amount { get; set; }

        public string SendingCurrency { get; set; }
        public string Identifier { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string StatusName { get; set; }
        public FaxingStatus Status { get; set; }
        public TransactionServiceType TransactionType { get; set; }
        public Agent.Models.Type Type { get; set; }
        public bool IsAwaitForApproval { get; set; }
        public int TotalCount { get; set; }

    }
}