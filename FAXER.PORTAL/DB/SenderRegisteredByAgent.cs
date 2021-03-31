using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SenderRegisteredByAgent
    {
        public int Id { get; set; }
        public int SenderId { get; set; }

        public int AgentId { get; set; }

        public bool IsAuxAgent { get; set; }
    }
}