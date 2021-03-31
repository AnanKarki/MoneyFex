using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PartnerCurrentCommisionViewModel
    {
        public const string BindProperty = "Id , Code ,Country , PartnerAgentId,PartnerId,PartnerAccountNo,AgentName ,City , AgentAccountNo, SendingCommission, ReceivingCommission, TransferSevice,CommissionType , CommissionDueDate";

        public int Id { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public int PartnerAgentId { get; set; }
        public int PartnerId { get; set; }
        public int PartnerAccountNo { get; set; }
        public string AgentName { get; set; }
        public string City { get; set; }
        public string AgentAccountNo { get; set; }
        public decimal? SendingCommission { get; set; }
        public decimal? ReceivingCommission { get; set; }
        public TransferService TransferSevice { get; set; }
        public CommissionType CommissionType { get; set; }
        public CommissionDueDate CommissionDueDate { get; set; }

    }
}