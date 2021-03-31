using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SMSFeelistViewModel
    {
        public const string BindProperty = "Id , CountryName,Country ,CountryCode ,SmsFee , IsDeleted  ";
        public int Id { get; set; }
        public string CountryName { get; set; }
        [Required]
        public string Country{ get; set; }

        [Required, DisplayName("Country Code")]
        public string CountryCode{ get; set; }

        [Required, DisplayName("Sms Fee")]
        public decimal SmsFee{ get; set; }
        public bool IsDeleted { get; set; }
    }

    public class SmsFeeVm {

        public string Country { get; set; }
        public List<SMSFeelistViewModel> SMSFeelistViewModels { get; set; }
    }
}