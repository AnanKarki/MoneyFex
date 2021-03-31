using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class FormAgentCommissionPaymentViewModel
    {
        public const string BindProperty = "Id , Month ,Year ,Country ,City ,AgentId , MFSCode, Status,CurrencySymbol ,TotalSentPayment, SentCommissionRate ,TotalSentCommission" +
            " , TotalReceivedPayment,ReceiverCommissionRate , TotalReceivedCommission,TotalCommission  ,IsVerified,AdminName";

        public int Id { get; set; }
        public Month Month { get; set; }
        public string Year { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int AgentId { get; set; }
        public string MFSCode { get; set; }
        public string Status { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal TotalSentPayment { get; set; }
        public decimal SentCommissionRate { get; set; }
        public decimal TotalSentCommission { get; set; }
        public decimal TotalReceivedPayment { get; set; }
        public decimal ReceiverCommissionRate { get; set; }
        public decimal TotalReceivedCommission { get; set; }
        public decimal TotalCommission { get; set; }
        public bool IsVerified { get; set; }
        public string AdminName { get; set; }
    }
}