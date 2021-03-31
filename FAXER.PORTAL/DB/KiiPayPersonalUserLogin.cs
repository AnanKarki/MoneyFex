using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public partial class KiiPayPersonalUserLogin
    {
        public int Id { get; set; }
        public int KiiPayPersonalUserInformationId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string ActivationCode { get; set; }
        public int LoginFailedCount { get; set; }

        public virtual KiiPayPersonalUserInformation KiiPayPersonalUserInformation { get; set; }
    }
}