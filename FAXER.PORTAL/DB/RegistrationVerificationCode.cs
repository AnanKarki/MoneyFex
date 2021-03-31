using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class RegistrationVerificationCode
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public string VerificationCode { get; set; }

        public RegistrationOf RegistrationOf { get; set; }

        public string Country { get; set; }
        public string PhoneNo { get; set; }


        public bool IsExpired { get; set; }

    }

    public enum RegistrationOf
    {

        Sender,
        KiiPayPersonal,
        KiiPayBusiness,
        Staff,
        Agent


    }
}