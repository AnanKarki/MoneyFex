using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SendingMoneyAbroadViewModel
    {
        public const string BindProperty = "Id ,CountryCode ,CountryPhoneCode ,PhoneNumber , PhoneNumberDropdown";
        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string PhoneNumberDropdown { get; set; }
    }
}