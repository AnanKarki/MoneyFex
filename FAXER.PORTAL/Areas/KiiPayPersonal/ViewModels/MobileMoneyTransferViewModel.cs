using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class MobileMoneyTransferViewModel
    {
        public const string BindProperty = " Id , RecentMobileNumber ,MobileNumber";
        public int Id { get; set; }
        public string RecentMobileNumber { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}