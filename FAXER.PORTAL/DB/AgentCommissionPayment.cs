using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentCommissionPayment
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public Month Month { get; set; }
        public string Year { get; set; }
        public decimal TotalSentPayment { get; set; }
        public decimal SendingCommissionRate { get; set; }
        public decimal TotalSentCommission { get; set; }
        public decimal TotalReceivedPayment { get; set; }
        public decimal ReceivingCommissionRate { get; set; }
        public decimal TotalReceivedCommission { get; set; }
        public decimal TotalCommission { get; set; }
        public string ReceiptNo { get; set; }
        public int VerifiedBy { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public virtual AgentInformation Agent { get; set; }
    }
}