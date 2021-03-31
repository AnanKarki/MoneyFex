using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffEmail
    {

        public int Id { get; set; }

        public int From_StaffId { get; set; }

        public int To_StaffId { get; set; }
        public string SubJect { get; set; }

        public string BankPaymentReference { get; set; }

        public EmailStatus EmailStatus { get; set; }
        public bool HasAttachment { get; set; }
        public string AttachmentURL { get; set; }
        public DateTime EmailSentDate { get; set; }
        public bool From_IsDeleted { get; set; }
        public bool To_IsDeleted { get; set; }
        public bool From_IsPermanentlyDeleted { get; set; }
        public bool To_IsPermanentlyDeleted { get; set; }
    }

    public enum EmailStatus
    {

        Draft,
        Sent,
        Delivered,
        Archived
    }
}