using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RegisteredAUXSenderViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string SenderName { get; set; }
        public string DateOfBirth { get; set; }
        public string GenderName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string AccountNo { get; set; }
        public string Date { get; set; }
        public DateTime? CreationDate { get; set; }
        public string StatusName { get; set; }
        public string AgentName { get; set; }
        public string AgentAccount { get; set; }

    }
}