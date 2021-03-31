using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class CashWithdrawalReceiptVM
    {

        public string ReceiptNo { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }

        public string AgentName { get; set; }
        public string AgentAccountNO { get; set; }
        public string WithDrawalType { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }
        public string WithdrawalCode { get; set; }
        public string AdminCodeGenerator { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public string Currency { get; set; }

        public bool IsWithdrawalByAgent { get; set; }




    }
}