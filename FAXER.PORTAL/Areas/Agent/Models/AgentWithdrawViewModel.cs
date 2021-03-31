using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentWithdrawViewModel
    {
        public const string BindProperty = "Id , AgentName ,AgentAccountNumber,AgentStaffName , WithdrawAmount ,ConfirmVerification";
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string AgentAccountNumber { get; set; }
        public string AgentStaffName { get; set; }
        public decimal WithdrawAmount { get; set; }
        public bool ConfirmVerification { get; set; }
    }
}