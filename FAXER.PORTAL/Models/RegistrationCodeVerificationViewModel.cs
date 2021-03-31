using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class RegistrationCodeVerificationViewModel
    {
        public const string BindProperty = "UserId,PhoneNo,VerificationCode,Message,Country,RegistrationOf";


        public int UserId { get; set; }

        public string PhoneNo { get; set; }

        public string VerificationCode { get; set; }


        public string Message { get; set; }

        public string Country { get; set; }
        
        public RegistrationOf RegistrationOf { get; set; }



    }

   
}