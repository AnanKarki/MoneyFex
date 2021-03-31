using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AccountingViewModel
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCountryCurrency { get; set; }
        public TransactionType TransactionType { get; set; }
        public Type Type { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Identifier { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount{ get; set; }
        public decimal Fee{ get; set; }
        public decimal MFRate{ get; set; }
        public decimal AgentRate{ get; set; }
        public decimal Margin{ get; set; }

    }
}