using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentNewDocumentViewModel
    {
        public int Id { get; set; }
        public DocumentType DocumentType { get; set; }

        public string ExpiryDate { get; set; }

        public string DocumentTitleName { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        public int AgentStaffId { get; set; }

        public DateTime UploadDateTime { get; set; }
        public string DocumentPhotoUrl { get; set; }
        public string AgentStaffName { get; set; }

        public string AgentStaffAccountNo { get; set; }
    }
}