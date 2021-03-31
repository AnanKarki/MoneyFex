using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentAccountBalance
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public decimal TotalBalance { get; set; }
        public DateTime? UpdateDateTime { get; set; }
    }
}