using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentCashWithdrawlViewModel
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string FirstLetter { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AccountNo { get; set; }
        public string WithdrawlType { get; set; }
        public string StaffName { get; set; }
        public string StaffCode { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string AccountBalance { get; set; }
        public string Receipt { get; set; }
    }
}