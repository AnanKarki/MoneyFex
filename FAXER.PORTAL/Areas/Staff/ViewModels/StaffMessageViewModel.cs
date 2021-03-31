using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class ComposeMessage
    { 
        public int MessageId { get; set; }
        public string MessageFrom { get; set; } 
        public string MessageTo { get; set; }

        public string MessageSubject { get; set; }
        public string TopUpCardNo { get; set; }

        public string Message_BankPaymentReference { get; set; }
        
        public DateTime MessageCreatedDate { get; set; }

    }
    public class InboxMessages
    {
        public string Messagefrom { get; set; }
        public string MessageSubject { get; set; }
        public string Message { get; set; }
        public string MessageDate { get; set; }
    }
    public class SentMessages
    {
        public string MessageFrom { get; set; }
        public string MessageTo { get; set; }
        public string MessageSubject { get; set; }

        public string Message { get; set; }

        public string MessageSentDate { get; set; }

    }
    public class DraftMessage
    {
        public string MessageFrom { get; set; }
        public string MessageTo { get; set; }
        public string MessageSubject { get; set; }

        public string Message { get; set; }

        public string MessageSentDate { get; set; }

    }
    public class ArchiveMessage
    {
        public string MessageFrom { get; set; }
        public string MessageTo { get; set; }
        public string MessageSubject { get; set; }

        public string Message { get; set; }

        public string MessageSentDate { get; set; }

    }

    
}