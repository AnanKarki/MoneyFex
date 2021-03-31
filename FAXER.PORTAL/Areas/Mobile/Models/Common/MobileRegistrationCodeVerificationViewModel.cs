using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileRegistrationCodeVerificationViewModel
    {

        public int UserId { get; set; }

        public string PhoneNo { get; set; }

        public string VerificationCode { get; set; }


        public string Message { get; set; }

        public string Country { get; set; }

        public RegistrationOf RegistrationOf { get; set; }

    }
}