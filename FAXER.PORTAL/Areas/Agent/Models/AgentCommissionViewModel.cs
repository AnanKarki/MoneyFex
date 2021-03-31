using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentCommissionViewModel
    {
        public string Year { get; set; }
        public Month Month { get; set; }

        public string AgentName { get; set; }
        public string AgentMFSCode { get; set; }

        public string TotalFaxedCommission { get; set; }

        public string TotalReceivedCommission { get; set; }

        public string FaxedAndReceivedCommission{get; set;}

        public string status { get; set; }

    }

    public enum AgentCommisionPaymentStatus {
        Paid , 
        UnPaid
    }
}