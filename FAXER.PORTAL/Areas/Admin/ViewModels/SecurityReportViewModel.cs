using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SecurityReportViewModel
    {
        public int Id{ get; set; }
        public  string Date{ get; set; }
        public  string Platform{ get; set; }
        public  string Type{ get; set; }
        public  string FaildLoginAttempt{ get; set; }
        public  string AssociatedUser{ get; set; }
        public  int InvalidBankCardAttempts{ get; set; }
    }
}