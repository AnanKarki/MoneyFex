using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentFaxMoneyInformation
    {
        [Key]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int NonCardTransactionId { get; set; }
        public string NameOfPayingAgent { get; set; }
        public int PayingAgentStaffId { get; set; }
        public DateTime Date { get; set; }
  

        public string FaxingCountry { get; set; }
        public string ReceivingCountry { get; set; }

        public virtual AgentInformation Agent { get; set; }
        public virtual FaxingNonCardTransaction NonCardTransaction { get; set; }


    }
}