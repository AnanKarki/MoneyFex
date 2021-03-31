using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayForServicesAbroadViewModel
    {
        public const string BindProperty = "Id ,CountryCode ,RecentlyPaidBusiness ,MobileNumber ";

        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        public string RecentlyPaidBusiness { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}