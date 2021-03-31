using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentUploadDocumentViewModel
    {
        public int Id { get; set; }
        public string AgentAccountNumber { get; set; }
        public string  DocumentName { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public string StaffName { get; set; }

        public DateTime DateTime { get; set; }

    }
}