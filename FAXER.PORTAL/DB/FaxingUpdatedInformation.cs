using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FaxingUpdatedInformation
    {

        [Key]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int NonCardTransactionId { get; set; }
        public string NameOfUpdatingAgent { get; set; }
        public DateTime Date { get; set; }
        public int UpdatingAgentAgentStaffId { get; set; }

        public virtual AgentInformation Agent { get; set; }
        public virtual FaxingNonCardTransaction NonCardTransaction { get; set; }
    }
}