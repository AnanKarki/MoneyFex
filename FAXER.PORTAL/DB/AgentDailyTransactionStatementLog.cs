using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentDailyTransactionStatementLog
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime StatementDate { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal AccountBalance { get; set; }

    }
}