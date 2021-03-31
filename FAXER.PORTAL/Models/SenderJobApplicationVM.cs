using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderJobApplicationVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string Postion { get; set; }
        public string CV { get; set; }
        public string SuppotingStatement { get; set; }
    }
}