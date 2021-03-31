using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class LargeFundMoneyTransferDetailVM
    {
        public string Year { get; set; }
        public Month month { get; set; }
        public List<LargeFundMoneyTransferListVM> LargeFundMoneyTransferList { get; set; }


    }

    public class LargeFundMoneyTransferListVM
    {

        public int TransactionId { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string AgentName { get; set; }
        public string AgentAccountNo { get; set; }
        public string AgentStaffLoginCode { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }


    }
}