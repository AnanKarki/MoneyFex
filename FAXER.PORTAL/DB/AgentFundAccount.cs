using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentFundAccount
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        //AdminSTaffId
        public int ApprovedBy { get; set; }
        public decimal Amount { get; set; }
        public string BankAccountNo { get; set; }
        public string BankSortCode { get; set; }
        public string PaymentReference { get; set; }
        public string Receipt { get; set; }
        public string AgentCountry { get; set; }
        public string City { get; set; }
        public AgentFundStatus Status { get; set; }
        public SenderPaymentMode SenderPaymentMode { get; set; }
        public string CardNumber { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsPaid { get; set; }
        public string stripe_ChargeId { get; set; }
        public CardProcessorApi? CardProcessorApi { get; set; }
    }

    public enum AgentFundStatus
    {
        Processing,
        Approved
    }

}