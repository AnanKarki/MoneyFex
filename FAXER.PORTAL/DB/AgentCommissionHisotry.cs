using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentCommissionHisotry
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int AgentId { get; set; }
        public Nullable<decimal> SendingRate { get; set; }
        public Nullable<decimal> ReceivingRate { get; set; }
        public TransferService TransferSevice { get; set; }
        public CommissionType CommissionType { get; set; }
        public CommissionDueDate CommissionDueDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}