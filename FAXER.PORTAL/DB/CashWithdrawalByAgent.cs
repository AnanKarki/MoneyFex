using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CashWithdrawalByAgent
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string AgentStaffName { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public int AgentStaffId { get; set; }
        public WithdrawalStatus Status { get; set; }
        public int? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDateTime { get; set; }

        public string ReceiptNo { get; set; }


        public virtual AgentInformation Agent { get; set; }
        
    }
}