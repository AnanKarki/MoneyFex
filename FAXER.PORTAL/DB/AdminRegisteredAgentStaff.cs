using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AdminRegisteredAgentStaff
    {

        public int Id { get; set; }
        public string BranchName { get; set; }
        public string  StaffName { get; set; }
        public string  TelePhoneNumber { get; set; }
        public string  EmailAddress { get; set; }
        public string  Password { get; set; }
        public string  Address { get; set; }
        public string AccountNumber { get; set; }
    }
}