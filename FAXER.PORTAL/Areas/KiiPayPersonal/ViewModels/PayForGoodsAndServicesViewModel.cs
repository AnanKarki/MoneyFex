using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayForGoodsAndServicesViewModel
    {
        public const string BindProperty = "Id ,RecentlyPaidBusiness ,BusinessMobileNumber ";

        public int Id { get; set; }
        public string RecentlyPaidBusiness { get; set; }
        [Required]
        public string BusinessMobileNumber { get; set; }
    }
}