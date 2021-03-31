using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SenderDetailsForExpotExcelViewModel
    {
        public string Email { get; set; }
        public string SenderName { get; set; }
        public string MFAccountNo { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string IsActive { get; set; }
    }
    public class TransactionDetailsForExpotExcelViewModel
    {
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string Sender { get; set; }
        public string SenderPhoneNo { get; set; }
        public string Receiver { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string Identifier { get; set; }
        public string DateTime { get; set; }
        public string Responsible { get; set; }
        public string Status { get; set; }
    }
}