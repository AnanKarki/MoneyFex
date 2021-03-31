using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentViewModels
    {
        public List<ViewRegisteredAgentsViewModel> LoginInformation { get; set; }
        public List<AgentRefundReceiptViewModel> RefundInformation { get; set; }
        public List<AgentTransctionActivityVm> TransactionStatement { get; set; }
        public List<AgentCommissionHisotryViewModel> CommissionHisotry { get; set; }

    }
}