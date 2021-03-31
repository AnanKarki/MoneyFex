using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentTransctionActivityVm
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string TransferMethod { get; set; }
        public string TransferType { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string identifier { get; set; }
        public string DateTime { get; set; }
        public string StaffName { get; set; }
    }
    public enum AgentTransferType
    {

        Transfer,
        Withdrawal
    }

}