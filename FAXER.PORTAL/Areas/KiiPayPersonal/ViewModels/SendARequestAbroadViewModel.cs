using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SendARequestAbroadViewModel
    {
        public const string BindProperty = "Id ,CountryCode ,RecentMobileNumber , CountryPhoneCode, MobileNumber";

        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        public string RecentMobileNumber { get; set; }

        public string CountryPhoneCode { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}