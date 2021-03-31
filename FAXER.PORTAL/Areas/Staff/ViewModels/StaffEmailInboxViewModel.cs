using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffEmailInboxViewModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Name { get; set; }
        public int ReceiverId { get; set; }
        public string Subject { get; set; }
        public string ShortBody { get; set; }
        public EmailStatus EmailStatus { get; set; }
        public bool HasAttachment { get; set; }
        public string EmailDate { get; set; }
        public string AttachmentURL { get; set; }

    }
}