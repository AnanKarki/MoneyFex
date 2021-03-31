using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class RefundNonCardFaxMoneyByAgent
    {
        public int Id { get; set; }
        [ForeignKey("FaxingNonCardTransaction")]
        public int NonCardTransaction_id { get; set; }

        [ForeignKey("AgentInformation")]
        public int Agent_id { get; set; }
        public string RefundReason { get; set; }

        public DateTime RefundedDate { get; set; }

        public string NameofRefunder { get; set; }

        public string ReceiptNumber { get; set; }

        public int PayingAgentStaffId { get; set; }
        public virtual FaxingNonCardTransaction FaxingNonCardTransaction { get; set; }

        public virtual AgentInformation AgentInformation { get; set; }
    }
}