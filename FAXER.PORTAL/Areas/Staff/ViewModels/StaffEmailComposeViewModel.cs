using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffEmailComposeViewModel
    {
        public const string BindProperty = "Id , FromEmailAddress , ToEmailAddress , ReceivingStaffId , Subject, AttachmentURL , BankPaymentReference , Draft," +
                                              " EmailList ";


        public int Id { get; set; }
        public string FromEmailAddress { get; set; }
        public string ToEmailAddress { get; set; }
        public int ReceivingStaffId { get; set; }
        public string Subject { get; set; }
        public string AttachmentURL { get; set; }
        public string BankPaymentReference { get; set; }

        public bool Draft { get; set; }
        public List<StaffEmails> EmailList { get; set; }
    }

    public class StaffEmails
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Telephone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string EmailId { get; set; }
    }
}